using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Pluralsight.CustomerService.Models;
using Pluralsight.CustomerService.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pluralsight.CustomerService.Dialogs
{
    //Las claves estan en el dashboard de Luis > publish  
    //Parametro opcional en LuisModel es el domain:""
    [LuisModel("f4195ce8-fd7a-44ae-9f75-ca925aeeb2c5", "5f206ec5731246729c8cf4a5bf642539", domain: "westus.api.cognitive.microsoft.com", Staging = true)]
    [Serializable]
    public class LUISDialog : LuisDialog<BugReport>
    {
        private readonly BuildFormDelegate<BugReport> NewBugReport;

        public LUISDialog(BuildFormDelegate<BugReport> newBugReport)
        {
            this.NewBugReport = newBugReport;
        }

        //Si Luis se queda sin opciones va a la Clase "None"
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Disculpa no entiendo a que te refieres.");
            context.Wait(MessageReceived);
        }

        //LuisIntent("Debeconicidir") debe coincidir, por que de esta manera se mapea con LUIS en la nube.
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            //**Colocar Punto Debug para localizar MEnsaje recivido
            context.Call(new GreetingDialog(), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
            //Se pueden agregar mas cosas como en MainDialog
        }

        [LuisIntent("NewBugReport")]

        public async Task BugReport(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<BugReport>(new BugReport(), this.NewBugReport, FormOptions.PromptInStart);
            context.Call<BugReport>(enrollmentForm, Callback);
        }

        [LuisIntent("QueryBugTypes")]
        public async Task QueryBugTypes(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(Entity => Entity.Type == "BugType"))
            {
                var value = entity.Entity.ToLower();
                if (Enum.GetNames(typeof(BugType)).Where(a => a.ToLower().Equals(value)).Count() > 0)
                {
                    var replyMessage = context.MakeMessage();
                    replyMessage.Text = "Si es un tipo de bug!";
                    var facebookMessage = new FacebookSendMessage();
                    facebookMessage.attachment = new FacebookAttachment();
                    facebookMessage.attachment.Type = FacebookAttachmentTypes.template;
                    facebookMessage.attachment.Payload = new FacebookPayload();
                    facebookMessage.attachment.Payload.TemplateType = FacebookTemplateTypes.generic;

                    var bugType = new FacebookElement();
                    bugType.Title = value;
                    switch (value)
                    {
                        case "seguridad":
                            bugType.ImageUrl = "https://c1.staticflickr.com/9/8604/16042227002_1d00e0771d_b.jpg";
                            bugType.Subtitle = "Contactese con el ejecutivo, Horario: L-V ; 9:00 - 18:00 hrs.";
                            break;
                        case "crash":
                            bugType.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/5/50/Windows_7_BSOD.png";
                            bugType.Subtitle = "This is a description of the crash bug type";
                            break;
                        case "power":
                            bugType.ImageUrl = "https://www.publicdomainpictures.net/pictures/10000/velka/1-1232525611kwZ7.jpg#.W3YsdlxzoSo.link";
                            bugType.Subtitle = "This is a description of the power bug type";
                            break;
                        case "rendimiento":
                            bugType.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/e/e5/High_Performance_Computing_Center_Stuttgart_HLRS_2015_07_Cray_XC40_Hazel_Hen_IO.jpg";
                            bugType.Subtitle = "This is a description of the performance bug type";
                            break;
                        case "usabilidad":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:03-Pau-DevCamp-usability-testing.jpg";
                            bugType.Subtitle = "This is a description of the usability bug type";
                            break;
                        case "errorgrave":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:Computer_bug.svg";
                            bugType.Subtitle = "This is a description of the serious bug type";
                            break;
                        case "otro":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:Symbol_Resin_Code_7_OTHER.svg";
                            bugType.Subtitle = "This is a description of the other bug type";
                            break;
                        default:
                            break;
                    }
                    facebookMessage.attachment.Payload.Elements = new FacebookElement[] { bugType };
                    replyMessage.ChannelData = facebookMessage;     //ChannelData es un Json especifico para ese canal.
                    await context.PostAsync(replyMessage);
                    context.Wait(MessageReceived);
                    return;
                }
                else
                {
                    await context.PostAsync("Disculpa pero ese no es un tipo de bug.");
                    context.Wait(MessageReceived);
                    return;
                }
            }
            await context.PostAsync("Disculpa pero ese no es un tipo de bug.");
            context.Wait(MessageReceived);
            return;
        }

    }
}