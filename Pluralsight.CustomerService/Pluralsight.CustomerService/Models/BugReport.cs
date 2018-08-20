using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluralsight.CustomerService.Models
{
    public enum BugType
    {
        Seguridad = 1,
        Crash = 2,
        Power = 3,
        Rendimiento = 4,
        Usabilidad = 5,
        ErrorGrave = 6,
        Otro = 7
    }

    public enum Reproductibility
    {
        Siempre= 1,
        Aveces = 2,
        Raramente = 3,
        Nunca = 4
    }
    
    [Serializable]
    public class BugReport
    {
        public string Title;
        [Prompt("Ingresa una descripcion para tu reporte")]
        public string Description;
        [Prompt("Cual es su primer nombre?")]
        public string FirstName;
        [Describe("Apellido")]
        public string LastName;
        [Prompt("Cual es la mejor fecha y hora para contactarle a traves de una llamada?")]
        public DateTime? BestTimeOfDayToCall;
        [Pattern("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$")]
        public string PhoneNumber;
        [Prompt("Por favor liste las areas de Bug que mejor describan su problema. {||}")]
        public List<BugType> Bug;
        public Reproductibility Reproduce;
        
        public static IForm<BugReport> BuildForm()
        {
            return new FormBuilder<BugReport>().Message("Por favor reporte el error").Build();

        }

    }
}