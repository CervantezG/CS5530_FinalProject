using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            // TODO: Works except for grade
            string fmt = "0000000";

            var query =
            from cl in db.Classes
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            join e in db.Enrolled
            on cl.ClassId equals e.ClassId
            join s in db.Students
            on e.UId equals s.UId
            where cl.Season == season
            && cl.Year == year
            && co.Number == num
            && co.Subject == subject
            select new
            {
                fname = s.FirstName,
                lname = s.LastName,
                uid = "u" + s.UId.ToString(fmt),
                dob = s.Dob,
                grade = e.Grade
            };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            // TODO: Test.  
            var query =
            from ac in db.AssignmentCategories
            join a in db.Assignments
            on ac.AssignmentCategoryId equals a.AssignmentCategoryId
            join cl in db.Classes
            on ac.ClassId equals cl.ClassId
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            join s in db.Submissions
            on a.AssignmentId equals s.AssignmentId
            into sub
            from q in sub.DefaultIfEmpty()
            where co.Subject == subject
            && co.Number == num
            && cl.Season == season
            && cl.Year == year
            && ((ac.Name == category)
                ||
                category == null
               )
            group q by new { assignmentName = a.Name, assignmentCategoryName = ac.Name, a.DueDate } into groupedAssignments
            select new
            {
                aname = groupedAssignments.Key.assignmentName,
                cname = groupedAssignments.Key.assignmentCategoryName,
                due = groupedAssignments.Key.DueDate,
                submissions = groupedAssignments.Count()
            };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query =
            from ac in db.AssignmentCategories
            join cl in db.Classes
            on ac.ClassId equals cl.ClassId
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            where co.Subject == subject
            && co.Number == num
            && cl.Season == season
            && cl.Year == year
            select new
            {
                name = ac.Name,
                weight = ac.Weight
            };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            bool result;

            try
            {
                var query =
                from cl in db.Classes
                join co in db.Courses
                on cl.CourseId equals co.CourseId
                where cl.Season == season
                && cl.Year == year
                && co.Number == num
                && co.Subject == subject
                select new
                {
                    cl.ClassId
                };

                // Create a new assignment category object.
                AssignmentCategories assignmentCategory = new AssignmentCategories
                {
                    Name = category,
                    Weight = (uint)catweight,
                    ClassId = query.FirstOrDefault().ClassId
                };

                db.AssignmentCategories.Add(assignmentCategory);
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
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            // TODO: Add side effect to calculate grade and update it
            bool result;

            try
            {
                var query =
                from cl in db.Classes
                join co in db.Courses
                on cl.CourseId equals co.CourseId
                join ac in db.AssignmentCategories
                on cl.ClassId equals ac.ClassId
                where cl.Season == season
                && cl.Year == year
                && co.Number == num
                && co.Subject == subject
                && ac.Name == category
                select new
                {
                    ac.AssignmentCategoryId,
                    cl.ClassId
                };

                // Create a new assignment category object.
                Assignments assignment = new Assignments
                {
                    Name = asgname,
                    Points = (uint)asgpoints,
                    Contents = asgcontents,
                    DueDate = asgdue,
                    AssignmentCategoryId = query.FirstOrDefault().AssignmentCategoryId
                };

                db.Assignments.Add(assignment);
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
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            // TODO : Test that other peoples assignments are not also visible
            string fmt = "0000000";

            var query =
            from cl in db.Classes
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            join ac in db.AssignmentCategories
            on cl.ClassId equals ac.ClassId
            join a in db.Assignments
            on ac.AssignmentCategoryId equals a.AssignmentCategoryId
            join sub in db.Submissions
            on a.AssignmentId equals sub.AssignmentId
            join st in db.Students
            on sub.UId equals st.UId
            where cl.Season == season
            && cl.Year == year
            && co.Number == num
            && co.Subject == subject
            && ac.Name == category
            && a.Name == asgname
            select new
            {
                fname = st.FirstName,
                lname = st.LastName,
                uid = "u" + st.UId.ToString(fmt),
                time = sub.SubmissionTime,
                score = sub.Score
            };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            // TODO: Add side effect to calculate grade and update it
            var query =
            from sub in db.Submissions
            join a in db.Assignments
            on sub.AssignmentId equals a.AssignmentId
            join ac in db.AssignmentCategories
            on a.AssignmentCategoryId equals ac.AssignmentCategoryId
            join cl in db.Classes
            on ac.ClassId equals cl.ClassId
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            where co.Subject == subject
            && co.Number == num
            && cl.Season == season
            && cl.Year == year
            && ac.Name == category
            && a.Name == asgname
            && sub.UId == uint.Parse(uid.Substring(1))
            select sub;

            Boolean result;

            try
            {
                foreach (Submissions row in query)
                {
                    row.Score = (uint)score;
                }

                db.SaveChanges();

                result = true;

                var classIdQuery =
                from a in db.Assignments
                join ac in db.AssignmentCategories
                on a.AssignmentCategoryId equals ac.AssignmentCategoryId
                join cl in db.Classes
                on ac.ClassId equals cl.ClassId
                join co in db.Courses
                on cl.CourseId equals co.CourseId
                where co.Subject == subject
                && co.Number == num
                && cl.Season == season
                && cl.Year == year
                && ac.Name == category
                && a.Name == asgname
                select new
                {
                    cl.ClassId
                };

                updateClassGrade(classIdQuery.FirstOrDefault().ClassId, uint.Parse(uid.Substring(1)));
            }
            catch (Exception e)
            {
                result = false;
            }

            return Json(new { success = result });
        }

        private void updateClassGrade(uint classId, uint uId)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
            from cl in db.Classes
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            where cl.Professor == uint.Parse(uid.Substring(1))
            select new
            {
                subject = co.Subject,
                number = co.Number,
                name = co.Name,
                season = cl.Season,
                year = cl.Year
            };

            return Json(query.ToArray());
        }


        /*******End code to modify********/

    }
}