// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace DotNetCoreSample;

public static class GroupByEqualitySample
{
    public static void MainTest()
    {
        var students = new StudentResult[]
        {
            new() { StudentName = "Ming", CourseName = "Chinese", Score = 80, },
            new()
            {
                StudentId = 1, StudentName = "Ming", CourseName = "English", Score = 60,
            },
            new()
            {
                StudentId = 2, StudentName = "Mike", CourseName = "English", Score = 70,
            },
            new() { StudentId = 1, CourseName = "Math", Score = 100, },
            new()
            {
                StudentName = "Mike", CourseName = "Chinese", Score = 60,
            },
        };
        var groups = students.GroupByEquality(x => new Student() { Id = x.StudentId, Name = x.StudentName },
            (s1, s2) => s1.Id == s2.Id || s1.Name == s2.Name, (x, k) =>
            {
                if (k.Id <= 0 && x.StudentId > 0)
                {
                    k.Id = x.StudentId;
                }
                else if (k.Id > 0 && x.StudentId <= 0)
                {
                    x.StudentId = k.Id;
                }

                if (k.Name.IsNullOrEmpty() && x.StudentName.IsNotNullOrEmpty())
                {
                    k.Name = x.StudentName;
                }
                else if (k.Name.IsNotNullOrEmpty() && x.StudentName.IsNullOrEmpty())
                {
                    x.StudentName = k.Name;
                }
            });
        foreach (var group in groups)
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"{group.Key.Id} {group.Key.Name}, Total score: {group.Sum(x => x.Score)}");
            foreach (var result in group)
            {
                Console.WriteLine($"{result.StudentId}  {result.StudentName}\n{result.CourseName}  {result.Score}");
            }
        }
    }



    private sealed class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Ming";
    }

    private sealed class StudentResult
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
