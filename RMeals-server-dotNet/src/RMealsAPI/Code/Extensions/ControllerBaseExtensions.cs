using Microsoft.AspNetCore.Mvc;

namespace RMealsAPI.Code.Extensions
{
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// Returns a HTTP 201 response, where the new object's URI is auto guessed (to the current request path the ID is appended).
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="id">The ID of the newly created object</param>
        /// <param name="value">The value we do wan't to return in the BODY (if omitted, ID is used)</param>
        /// <returns></returns>
        public static IActionResult Created(this ControllerBase controller, object id, object value = null)
        {
            value = value ?? id;

            var putUrl = controller.HttpContext.Request.Scheme + "://" + controller.HttpContext.Request.Host + controller.HttpContext.Request.Path;
            if (!putUrl.EndsWith("/")) putUrl += "/";

            var objectGetUrl = putUrl + id.ToString();

            return controller.Created(objectGetUrl.ToString(), value);            
        }

    }

}