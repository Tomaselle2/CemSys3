using CemSys3.DTOs.SweetAlert;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace CemSys3.Helpers.Mensajes
{
    public static class TempDataExtensions
    {
        public static void SetSweetAlert(this ITempDataDictionary tempData, SweetAlertDTO alert)
        {
            tempData["SweetAlert"] = JsonSerializer.Serialize(alert);
        }

        public static SweetAlertDTO? GetSweetAlert(this ITempDataDictionary tempData)
        {
            if (tempData["SweetAlert"] == null) return null;

            return JsonSerializer.Deserialize<SweetAlertDTO>(
                tempData["SweetAlert"].ToString()
            );
        }
    }
}
