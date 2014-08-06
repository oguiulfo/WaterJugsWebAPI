using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WaterJugsService;
using WaterJugsService.Models;

namespace WaterJugsWebAPI.Controllers
{
    /// <summary>
    /// This is an example of a Web API service to be used to get solutions to the Water Jugs problem
    /// </summary>
    [RoutePrefix("WaterJugsService")]
    public class ServiceController : ApiController
    {
        /// <summary>
        /// Submit constraints that will be used to generate a solution to the Water Jugs problem
        /// </summary>
        [HttpPost]
        [Route("Submit")]
        public IHttpActionResult Submit(WaterJugConstraints model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState));
            }

            if (model.JugA_Max > 0 || model.JugB_Max > 0 || model.TargetGallons > 0)
            {
                // This is the business logic for a brute force search using the "repeatedly fill one jug" method - includes GCD check
                // Other methods: (using Production Rules, BFS or DFS): http://kartikkukreja.wordpress.com/2013/10/11/water-jug-problem/
                try
                {
                    var gcd = System.Numerics.BigInteger.GreatestCommonDivisor(model.JugA_Max, model.JugB_Max);
                    var isMultiple = model.TargetGallons % gcd == 0;

                    if (isMultiple)
                    {
                        var ds = new BuildWaterJugResultSet(model);
                        var stepsA = Task.Run(() => ds.CheckAtoB()).Result;
                        var stepsB = Task.Run(() => ds.CheckBtoA()).Result;

                        if (stepsA.Count < stepsB.Count)
                            return Ok(stepsA);
                        else
                            return Ok(stepsB);
                    }
                    else
                    {
                        var error = string.Format("ERROR: There is no solution. The greatest common divisor of {0} and {1} is not a multiple of {2}",
                            model.JugA_Max, model.JugB_Max, model.TargetGallons);

                        return BadRequest(error);
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
            {
                return BadRequest("ERROR: All values must be greater than zero.");
            }
        }
    }
}