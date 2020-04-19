using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
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


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            //TODO: Test adding grade
            var query =
            from cl in db.Classes
            join e in db.Enrolled
            on cl.ClassId equals e.ClassId
            join co in db.Courses
            on cl.CourseId equals co.CourseId
            where e.UId == uint.Parse(uid.Substring(1))
            select new
            {
                subject = co.Subject,
                number = co.Number,
                name = co.Name,
                season = cl.Season,
                year = cl.Year,
                grade = e.Grade == null ? "--" : e.Grade
            };


            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            // TODO: Test with a submission
            // TODO: Fix bug where only submitted assignments show up. Do this last.  Implement everything else first.
            var tempTable =
            (from ac in db.AssignmentCategories
             join a in db.Assignments
             on ac.AssignmentCategoryId equals a.AssignmentCategoryId
             join cl in db.Classes
             on ac.ClassId equals cl.ClassId
             join co in db.Courses
             on cl.CourseId equals co.CourseId
             join e in db.Enrolled
             on cl.ClassId equals e.ClassId
             where co.Subject == subject
             && co.Number == num
             && cl.Season == season
             && cl.Year == year
             && e.UId == uint.Parse(uid.Substring(1))
             select new
             {
                 a.AssignmentId,
                 aname = a.Name,
                 cname = ac.Name,
                 due = a.DueDate
             }).Distinct();

            object[] results = new object[tempTable.Count()];

            int i = 0;

            foreach (var row in tempTable.ToArray())
            {
                results[i] = new
                {
                    aname = row.aname,
                    cname = row.cname,
                    due = row.due,
                    score = getSubmissionScore(row.AssignmentId, int.Parse(uid.Substring(1)))
                };
                ++i;
            }


            //row.score = (uint?)99; // 
            //db.SaveChanges();


            //var query =
            //from tt in tempTable
            //join sub in db.Submissions
            //on tt.AssignmentId equals sub.AssignmentId
            //into final
            //from q in final.DefaultIfEmpty()
            //select new
            //{
            //    tt.aname,
            //    tt.cname,
            //    tt.due,
            //    score = q.Score == null ? null : q.Score
            //};




            //var query =
            //from ac in db.AssignmentCategories
            //join a in db.Assignments
            //on ac.AssignmentCategoryId equals a.AssignmentCategoryId
            //join cl in db.Classes
            //on ac.ClassId equals cl.ClassId
            //join co in db.Courses
            //on cl.CourseId equals co.CourseId
            //join s in db.Submissions
            //on a.AssignmentId equals s.AssignmentId
            //into sub
            //from q in sub.DefaultIfEmpty()
            //where co.Subject == subject
            //&& co.Number == num
            //&& cl.Season == season
            //&& cl.Year == year
            //select new
            //{
            //    aname = a.Name,
            //    cname = ac.Name,
            //    due = a.DueDate,
            //    score = q.Score
            //};

            //foreach (var row in tempTable)
            //{
            //    System.Diagnostics.Debug.WriteLine("");
            //    System.Diagnostics.Debug.WriteLine(row.aname);
            //    System.Diagnostics.Debug.WriteLine(row.cname);
            //    System.Diagnostics.Debug.WriteLine(row.score.ToString());
            //    System.Diagnostics.Debug.WriteLine("");
            //}

            //var query =
            //from a in db.Assignments
            //join ac in db.AssignmentCategories
            //on a.AssignmentCategoryId equals ac.AssignmentCategoryId
            //join cl in db.Classes
            //on ac.ClassId equals cl.ClassId
            //join co in db.Courses
            //on cl.CourseId equals co.CourseId
            //join s in db.Submissions
            //on a.AssignmentId equals s.AssignmentId
            //into sub
            //from q in sub.DefaultIfEmpty()
            //where cl.Season == season
            //&& cl.Year == year
            //&& co.Number == num
            //&& co.Subject == subject
            //select new
            //{
            //    aname = a.Name,
            //    cname = ac.Name,
            //    due = a.DueDate,
            //    score = (uint?)q2.Score
            //};

            return Json(results);
        }

        private uint? getSubmissionScore(uint assignmentId, int uId)
        {
            var query =
            from sub in db.Submissions
            where sub.AssignmentId == assignmentId
            && sub.UId == uId
            select new
            {
                sub.Score
            };

            uint? score;

            if (query.Count() == 0)
            {
                score = null;
            }
            else
            {
                score = query.FirstOrDefault().Score;
            }

            return score;
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            // TODO: Test where there is no assignment and then again where there is an assignment
            Boolean result = true;

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
            into q
            where ac.Name == category
            && co.Subject == subject
            && co.Number == num
            && cl.Season == season
            && cl.Year == year
            && a.Name == asgname
            select new
            {
                AssignmentId = a.AssignmentId,
                sub = q.Where(sub => sub.UId == uint.Parse(uid.Substring(1))).Select(sub => new { uId = sub.UId })

            };

            Boolean firstSubmission = true;

            foreach (var row in query)
            {
                if (row.sub.Count() > 0)
                {
                    firstSubmission = false;
                }
            }

            try
            {
                if (firstSubmission)
                {
                    // Create a new course object.
                    Submissions submitionOfAssignment = new Submissions
                    {
                        UId = uint.Parse(uid.Substring(1)),
                        AssignmentId = query.FirstOrDefault().AssignmentId,
                        Score = 0,
                        SubmissionTime = DateTime.Now,
                        Contents = contents
                    };

                    db.Submissions.Add(submitionOfAssignment);
                    db.SaveChanges();

                    result = true;
                }
                else
                {
                    var existingSubmissionQuery =
                    from s in db.Submissions
                    where s.AssignmentId == query.FirstOrDefault().AssignmentId
                    && s.UId == uint.Parse(uid.Substring(1))
                    select s;

                    foreach (var row in existingSubmissionQuery)
                    {
                        row.Contents = contents;
                        row.SubmissionTime = DateTime.Now;
                    }

                    db.SaveChanges();
                }

            }
            catch (Exception)
            {
                result = false;
            }

            return Json(new { success = result });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
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
                Enrolled classEnrollment = new Enrolled
                {
                    UId = uint.Parse(uid.Substring(1)),
                    ClassId = query.FirstOrDefault().ClassId
                };

                db.Enrolled.Add(classEnrollment);
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
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            // TODO: Test this when a student has some grades.
            var query =
            from e in db.Enrolled
            where e.UId == uint.Parse(uid.Substring(1))
            && e.Grade != null
            select new
            {
                gradePoints = convertLetterGradeToPoint(e.Grade.Trim())
            };

            double gpa;

            int gradePointsCount = query.Count();
            if (gradePointsCount > 0)
            {
                double gradePointsSum = 0;

                foreach (var row in query)
                {
                    gradePointsSum += row.gradePoints;
                }

                gpa = gradePointsSum / gradePointsCount;
            }
            else
            {
                gpa = 0.0;
            }

            return Json(new { gpa = gpa });
        }

        private static double convertLetterGradeToPoint(string letterGrade)
        {
            double gradePoint;

            if (letterGrade.Equals("A"))
            {
                gradePoint = 4.0;
            }
            else if (letterGrade.Equals("A-"))
            {
                gradePoint = 3.7;
            }
            else if (letterGrade.Equals("B+"))
            {
                gradePoint = 3.3;
            }
            else if (letterGrade.Equals("B"))
            {
                gradePoint = 3.0;
            }
            else if (letterGrade.Equals("B-"))
            {
                gradePoint = 2.7;
            }
            else if (letterGrade.Equals("C+"))
            {
                gradePoint = 2.3;
            }
            else if (letterGrade.Equals("C"))
            {
                gradePoint = 2.0;
            }
            else if (letterGrade.Equals("C-"))
            {
                gradePoint = 1.7;
            }
            else if (letterGrade.Equals("D+"))
            {
                gradePoint = 1.3;
            }
            else if (letterGrade.Equals("D"))
            {
                gradePoint = 1.0;
            }
            else if (letterGrade.Equals("D-"))
            {
                gradePoint = 0.7;
            }
            // Case when letterGrade.Equals("E")
            else
            {
                gradePoint = 0.0;
            }
            return gradePoint;
        }

        /*******End code to modify********/

    }
}