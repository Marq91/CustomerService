using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pluralsight.CustomerService.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            //Punto debug para el saludo del bot
            await context.PostAsync("Hola soy Jhon Bot");
            await Respond(context);

            context.Wait(MessageReceivedAsync);
        }

        //Codigo nuevo, el antiguo sera utilizado***
        //private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    throw new NotImplementedException();
        //}

        private static async Task Respond(IDialogContext context)
        {
            var userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);
            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("Cual es su nombre?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync(String.Format("Hola {0}. Como puedo ayudarte hoy?", userName));
            }
        }

        //IAwaitable<object> ....virtual
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var userName = String.Empty;
            var getName = false;
            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
                await Respond(context);
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                context.Done(message);
            }
            
        }   //context.Wait(MessageReceivedAsync); //podria ir aqui

    }
}