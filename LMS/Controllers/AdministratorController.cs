﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            var query =
            from c in db.Courses
            where c.Subject == subject
            select new
            {
                number = c.Number,
                name = c.Name
            };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var query =
            from p in db.Professors
            where p.Department == subject
            select new
            {
                lname = p.LastName,
                fname = p.FirstName,
                uid = p.UId
            };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            Boolean result;

            try
            {
                // Create a new course object.
                Courses course = new Courses
                {
                    Subject = (string)subject,
                    Number = (int)number,
                    Name = (string)name
                };

                db.Courses.Add(course);
                db.SaveChanges();

                result = true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                result = false;
            }

            return Json(new { success = result });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            Boolean result = true;

            // Check to see if another class occupies the same location at the same time
            var openLocationQuery =
            from c in db.Classes
            where (c.Location == location)
            && (c.Season == season)
            && (c.Year == year)
            && end.TimeOfDay > c.StartTime
            && start.TimeOfDay < c.EndDtime
            select new { courseId = c.ClassId };

            foreach (var pRow in openLocationQuery)
            {
                result = false;
                break;
            }

            try
            {
                if (result)
                {
                    // Get CourseId using subject and number
                    var query =
                    from c in db.Courses
                    where (c.Subject == subject)
                    && (c.Number == number)
                    select new { courseId = c.CourseId };

                    uint? courseId = null;
                    foreach (var cRow in query)
                    {
                        courseId = cRow.courseId;
                        break;
                    }

                    // Create a new class object.
                    Classes newClass = new Classes
                    {
                        CourseId = (uint)courseId,
                        Season = (string)season,
                        Year = (uint)year,
                        StartTime = (TimeSpan)start.TimeOfDay,
                        EndDtime = (TimeSpan)end.TimeOfDay,
                        Location = (string)location,
                        Professor = uint.Parse(instructor)
                    };

                    db.Classes.Add(newClass);
                    db.SaveChanges();
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                result = false;
            }

            return Json(new { success = result });
        }


        /*******End code to modify********/

    }
}