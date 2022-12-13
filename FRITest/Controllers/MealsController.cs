using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer;
using Microsoft.EntityFrameworkCore;
using FRITest.Models;
using System.Diagnostics;
using System.Net.Http;          //HttpClient, HttpResponseMessage
using System.Net.Http.Json;     //HttpClientJsonExtensions
using System.Threading;         //CancellationToken
using System.Threading.Tasks;   //Task
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using NuGet.Protocol.Plugins;
using System.Windows;

namespace FRITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly FritestContext _context;

        public MealsController(FritestContext context)
        {
            _context = context;
        }


        [Route("GetMealsPaginados/")]
        [HttpGet]
        public List<Meal> GetSystems(int idUser,int page)
        {
            int i = 0;
            while (i<31)
            {
                StartProcess(idUser);
                i++;
            }


            return ListarMeals(idUser, page);
        }

            // GET: api/Meals
            [HttpGet]
        public async Task<ActionResult<IEnumerable<Meal>>> GetMeals()
        {
            
            return await _context.Meals.ToListAsync();
        }

        // GET: api/Meals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Meal>> GetMeal(string id)
        {
            
            var meal = await _context.Meals.FindAsync(id);
            
            if (meal == null)
            {
                return NotFound();
            }

            return meal;
        }

        // PUT: api/Meals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeal(string id, Meal meal)
        {
            if (id != meal.idMeal)
            {
                return BadRequest();
            }

            _context.Entry(meal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MealExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Meals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Meal>> PostMeal(Meal meal)
        {
            _context.Meals.Add(meal);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MealExists(meal.idMeal))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMeal", new { id = meal.idMeal }, meal);
        }

        // DELETE: api/Meals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(string id)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null)
            {
                return NotFound();
            }

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MealExists(string id)
        {
            return _context.Meals.Any(e => e.idMeal == id);
        }



        void StartProcess(int idUser)
        {
            AddUser(idUser);

        }

        void AddUser(int idUser)
        {

            if (!_context.Users.Any(e => e.IdUser == idUser))
            {
                User newUser = new User();
                newUser.IdUser = idUser;
                newUser.UserName = "Usuario " + idUser;
                this._context.Users.Add(newUser);
                this._context.SaveChanges();
            }

            try
            {
                FillMeals(idUser);
            }catch(Exception e){
                
            }
            

        }

        void FillMeals(int idUser)
        {
            Console.WriteLine($"Application Started at {DateTime.UtcNow}");
            using (var httpClient = new HttpClient())
            {
                MealsL meal = null;


                var task = httpClient.GetAsync("https://www.themealdb.com/api/json/v1/1/random.php");
                task.Wait();
                //var taskResponse1 = task.Result.Content.ReadFromJsonAsync<Meal>();
                var taskResponse1 = task.Result.Content.ReadAsStringAsync();

                taskResponse1.Wait();
                String R = taskResponse1.Result;
                Debug.WriteLine(R);

                //  meal = taskResponse1.Result;
                meal = JsonSerializer.Deserialize<MealsL>(R); //JsonConvert.DeserializeObject<Meteorologia>(json);

                Meal NewMeal = meal.meals[0];

                addMeal(NewMeal, idUser);

            }
            Debug.WriteLine($"Application Ended at {DateTime.UtcNow}");
        }

        void addMeal(Meal meal,int idUser)
        {
            if(!_context.Meals.Any(e => e.idMeal == meal.idMeal))
            {
                this._context.Meals.Add(meal);
                this._context.SaveChanges();
            }
            

            AddMealUser(idUser, meal.idMeal);

        }

        void AddMealUser(int idUser, string idMeal)
        {
            
            var MU = from m in _context.MealsUsers
                        where m.IdUser == idUser && m.IdMeal == idMeal 
                        select m;

            int a = MU.ToList().ToArray().Length;


            int counter = (from m in _context.MealsUsers
                                    where m.IdUser == idUser
                                    select m).Count();

            /*Validación que no exista relación platillo-usuario y que no se exceda 30 veces*/
            if (a == 0 && counter<=29)
            {
            MealsUser newMealUser = new MealsUser();
                            newMealUser.IdUser = idUser;
                            newMealUser.IdMeal = idMeal;
                            this._context.MealsUsers.Add(newMealUser);
                            this._context.SaveChanges();
            }        
        }

        List<Meal> ListarMeals(int idUser, int page)
        {
            var query = from Meals in _context.Meals
                        join MealUsers in _context.MealsUsers on Meals.idMeal equals MealUsers.IdMeal
                        where MealUsers.IdUser == idUser
                        select Meals;


            int pageSize = 10;
            page = (page <= 0) ? 1 : page;

            var skip = (page - 1) * pageSize;

             

            //List<Meal> MealsList= query.ToList();
            List<Meal> MealsList = query.Skip(skip).Take(pageSize).ToList();
            /*
            foreach (var M in query)
            {
                
                Debug.WriteLine(M.idMeal);
            }                   */
            return MealsList;
        }
        

    }
}
;