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
        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("Hi I'm Jhon Bot");
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        //Codigo nuevo, el antiguo sera utilizado***
        //private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    throw new NotImplementedException();
        //}

        //IAwaitable<object>
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var userName = String.Empty;
            var getName = false;

            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
            }

            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync(String.Format("Hi {0}. Howcan I help you today?", userName));
            }
            context.Wait(MessageReceivedAsync);
        }   //context.Wait(MessageReceivedAsync); //podria ir aqui

    }
}