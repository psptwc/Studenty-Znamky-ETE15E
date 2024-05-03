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
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Student> studentList = new List<Student>();

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
                        studentList.Add(Student.AddStudent());
                        break;
                    case 'v':
                        Console.Clear();
                        foreach (Student student in studentList)
                            Console.WriteLine($"{student.StudentID} {student.Name} {student.Lastname}");
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
