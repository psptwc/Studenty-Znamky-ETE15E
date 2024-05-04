using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace StudentyZnamky
{
    public class Student
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public int StudentID { get; set; }
        public Student(string name, string lastname, int studentID)
        {
            Name = name;
            Lastname = lastname;
            StudentID = studentID;
        }

        /*public static void WriteLine(Student student)
        {
            Console.WriteLine($"{student.StudentID} {student.Name} {student.Lastname}");
        }*/

        public static Student AddStudent()
        {
            Console.Write("Zadejte jmeno: ");
            string name = Console.ReadLine();
            Console.Write("Zadejte prijmeni: ");
            string lastname = Console.ReadLine();
            Console.Write("Zadejte ID studenta: ");
            int studentID = int.Parse(Console.ReadLine());

            return new Student(name, lastname, studentID);
        }
    }

    public class Subject
    {
        public string Name { get; set; }
        public int SubjectID { get; set; }
        public int Grade { get; set; }

        public Subject(string name, int subjectID, int grade)
        {
            Name = name;
            SubjectID = subjectID;
            Grade = grade;
        }

        public static Subject AddSubject()
        {
            Console.Write("Zadejte nazev: ");
            string name = Console.ReadLine();
            Console.Write("Zadejte ID predmeta: ");
            int subjectID = int.Parse(Console.ReadLine());
            Console.Write("Zadejte znamku: ");
            int grade = int.Parse(Console.ReadLine());

            return new Subject(name, subjectID, grade);
        }
    }

    public struct StudentSubject
    {
        public Student Student { get; set; }
        public Subject Subject { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //Dictionary<Student, List<Subject>> studentSubjects = new Dictionary<Student, List<Subject>>();
            List<StudentSubject> studentSubject = new List<StudentSubject>();
            //List<Student> studentList = new List<Student>();

            char answer;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Pridat studenta [p]");
                Console.WriteLine("Vypsat studenty [v]");
                Console.WriteLine("To quit [q]");
                answer = char.ToLower(Console.ReadKey().KeyChar);

                switch (answer)
                {
                    case 'p':
                        Console.Clear();
                        studentSubject.Add(new StudentSubject { 
                            Student = Student.AddStudent(), 
                            Subject = Subject.AddSubject() });
                        break;
                    case 'v':
                        Console.Clear();
                        foreach (StudentSubject i in studentSubject)
                            Console.WriteLine($"{i.Student.StudentID} {i.Student.Name} {i.Student.Lastname}\n" +
                                              $"{i.Subject.SubjectID} {i.Subject.Name} {i.Subject.Grade}\n");
                        Console.ReadKey();
                        break;
                    case 'q':
                        break;
                    default:
                        break;
                }
                if (answer == 'q')
                    break;
            }
        }
    }
}
