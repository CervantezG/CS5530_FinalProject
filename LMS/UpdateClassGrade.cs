using LMS.Models.LMSModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS
{
    public static class UpdateClassGrade
    {
        public static void updateClassGrade(Team11LMSContext db, uint classId, uint uId)
        {
            // Create twos dictionary for each assignment category to possible points and earned points
            Dictionary<string, uint> possiblePoints = new Dictionary<string, uint>();
            Dictionary<string, uint> earnedPoints = new Dictionary<string, uint>();

            // Create a dictionary for each assignment category weight
            Dictionary<string, uint> categoryWeights = new Dictionary<string, uint>();

            // Write a query to return "Assignment Category", "Category Weight", "Possible Points", and "EarnedPoint" (zero if empty)
            var query =
            (from ac in db.AssignmentCategories
             join cl in db.Classes
             on ac.ClassId equals cl.ClassId
             join a in db.Assignments
             on ac.AssignmentCategoryId equals a.AssignmentCategoryId
             where cl.ClassId == classId
             select new
             {
                 assignmentName = a.Name,
                 categoryName = ac.Name,
                 AssignmentCategoryId = ac.AssignmentCategoryId,
                 categoryWeight = ac.Weight,
                 possiblePoints = a.Points
             }).Distinct();

            // Iterate over each row in query to sum total points and possible points to 
            foreach (var row in query)
            {
                if (!possiblePoints.ContainsKey(row.categoryName))
                {
                    possiblePoints.Add(row.categoryName, row.possiblePoints);
                    categoryWeights.Add(row.categoryName, row.categoryWeight);
                    earnedPoints.Add(row.categoryName, getCategoryEarnedPoints(db, row.AssignmentCategoryId, uId));
                }
                else
                {
                    possiblePoints[row.categoryName] = possiblePoints[row.categoryName] + row.possiblePoints;
                }

                //System.Diagnostics.Debug.WriteLine("");
                //System.Diagnostics.Debug.WriteLine(row.assignmentName);
                //System.Diagnostics.Debug.WriteLine(row.categoryName);
                //System.Diagnostics.Debug.WriteLine(row.categoryWeight);
                //System.Diagnostics.Debug.WriteLine(row.possiblePoints);
                //System.Diagnostics.Debug.WriteLine("");
            }

            //System.Diagnostics.Debug.WriteLine("------------------------------");

            double weightedEarnedPercentage = 0;

            double categoryWeightSum = 0;

            double percentage;

            foreach (string key in possiblePoints.Keys)
            {
                percentage = (double)earnedPoints[key] / (double)possiblePoints[key];
                weightedEarnedPercentage = weightedEarnedPercentage + (percentage * categoryWeights[key]);

                categoryWeightSum = categoryWeightSum + categoryWeights[key];
                //System.Diagnostics.Debug.WriteLine(key);
                //System.Diagnostics.Debug.WriteLine(possiblePoints[key]);
                //System.Diagnostics.Debug.WriteLine(categoryWeights[key]);
                //System.Diagnostics.Debug.WriteLine(earnedPoints[key]);
                //System.Diagnostics.Debug.WriteLine("");
            }

            var enrolledQuery =
            from e in db.Enrolled
            where e.ClassId == classId
            && e.UId == uId
            select e;

            try
            {
                foreach (Enrolled row in enrolledQuery)
                {
                    row.Grade = getLetterGradeFromPercentage(weightedEarnedPercentage * (100 / categoryWeightSum));
                }

                System.Diagnostics.Debug.WriteLine(weightedEarnedPercentage * (100 / categoryWeightSum));


                db.SaveChanges();
            }
            catch (Exception e)
            {
                // Actually do this in a loop and continue until a success happens
            }
        }

        private static String getLetterGradeFromPercentage(double percentage)
        {
            String grade = "";
            if (percentage >= 93)
            {
                grade = "A";
            }
            else if (percentage < 93 && percentage >= 90)
            {
                grade = "A-";
            }
            else if (percentage < 90 && percentage >= 87)
            {
                grade = "B+";
            }
            else if (percentage < 87 && percentage >= 83)
            {
                grade = "B";
            }
            else if (percentage < 83 && percentage >= 80)
            {
                grade = "B-";
            }
            else if (percentage < 80 && percentage >= 77)
            {
                grade = "C+";
            }
            else if (percentage < 77 && percentage >= 73)
            {
                grade = "C";
            }
            else if (percentage < 73 && percentage >= 70)
            {
                grade = "C-";
            }
            else if (percentage < 70 && percentage >= 67)
            {
                grade = "D+";
            }
            else if (percentage < 67 && percentage >= 63)
            {
                grade = "D";
            }
            else if (percentage < 63 && percentage >= 60)
            {
                grade = "D-";
            }
            else if (percentage < 60)
            {
                grade = "E";
            }
            return grade;
        }

        private static uint getCategoryEarnedPoints(Team11LMSContext db, uint assignmentCategoryId, uint uId)
        {
            var query =
            from sub in db.Submissions
            join a in db.Assignments
            on sub.AssignmentId equals a.AssignmentId
            where sub.UId == uId
            && a.AssignmentCategoryId == assignmentCategoryId
            select new
            {
                sub.Score
            };

            uint earnedPoints = 0;

            foreach (var row in query)
            {
                earnedPoints = earnedPoints + row.Score;
            }

            return earnedPoints;

        }
    }
}
