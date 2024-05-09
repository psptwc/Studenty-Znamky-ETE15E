using System;
using System.IO;
using System.Collections.Generic;
using static StudentyZnamky.Program;
using System.Diagnostics;

namespace StudentyZnamky
{
    internal class Program
    {
        public struct Student
        {
            public int StudentID { get; set; }
            public string Name { get; set; }
            public string Lastname { get; set; }
            public Student(int studentID, string name, string lastname)
            {
                StudentID = studentID;
                Name = name;
                Lastname = lastname;
            }


            public static Student AddStudent()
            {
                int studentID;
                Console.Write("Zadejte ID studenta: ");
                while (!int.TryParse(Console.ReadLine(), out studentID))
                {
                    Console.WriteLine("Musi to byt cislo");
                    Console.Write("Zadejte ID studenta: ");
                }
                Console.Write("Zadejte jmeno: ");
                string name = Console.ReadLine();
                Console.Write("Zadejte prijmeni: ");
                string lastname = Console.ReadLine();

                return new Student(studentID, name, lastname);
            }

            public static void PrintStudent(Student student)
            {
                Console.Write($"{student.StudentID} {student.Name} {student.Lastname} ");
            }
        }

        public struct Subject
        {
            public int SubjectID { get; set; }
            public string Name { get; set; }

            public Subject(int subjectID, string name)
            {
                SubjectID = subjectID;
                Name = name;
            }

            public static Subject AddSubject()
            {
                int subjectID;
                Console.Write("Zadejte ID predmeta: ");
                while (!int.TryParse(Console.ReadLine(), out subjectID))
                {
                    Console.WriteLine("Musi to byt cislo");
                    Console.Write("Zadejte ID predmeta: ");
                }
                Console.Write("Zadejte nazev predmeta: ");
                string name = Console.ReadLine();

                return new Subject(subjectID, name);
            }

            public static void PrintSubject(Subject subject)
            {
                Console.Write($"{subject.SubjectID} {subject.Name} ");
            }
        }

        public struct StudentSubject
        {
            public Student Student { get; set; }
            public Subject Subject { get; set; }
            public int Grade { get; set; }

            public StudentSubject(Student student, Subject subject, int grade)
            {
                Student = student;
                Subject = subject;
                Grade = grade;
            }

            public static int AddGrade()
            {
                int grade;

                Console.Write("Zadejte znamku: ");
                while (!int.TryParse(Console.ReadLine(), out grade) || grade > 4 || grade < 1)
                {
                    Console.WriteLine("Nezadal jste cislo nebo je spatne");
                    Console.Write("Zadejte znamku: ");
                }

                return grade;
            }
        }

        static List<string[]> ReadFromCSV(string rootPath)
        {
            List<string[]> data = new List<string[]>();

            try
            {
                using (var reader = new StreamReader(rootPath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');
                        data.Add(values);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Soubor nebyl nalezen");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Chyba pri cteni soubora " + ex.Message);
            }
            return data;
        }

        static void Main(string[] args)
        {
            List<StudentSubject> studentSubject = new List<StudentSubject>();
            List<string[]> data;

            char answer;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Pridat studenta [e]");
                Console.WriteLine("Nacist csv soubor studenty [c]");
                Console.WriteLine("Nacist csv soubor predmety [x]");
                Console.WriteLine("------------------------------");
                Console.WriteLine("Vypsat data [v]");
                Console.WriteLine("Vypsat studenta [s]");
                Console.WriteLine("Vypsat predmet [p]");
                Console.WriteLine("------------------");
                //Console.WriteLine("Vymazat jednu znamku studenta [h]");
                //Console.WriteLine("Vymazat studenta [j]");
                //Console.WriteLine("Vymazat predmet [k]");
                //Console.WriteLine("Zmenit znamku studenta [z]");
                Console.WriteLine("To quit [q]");
                answer = char.ToLower(Console.ReadKey().KeyChar);

                switch (answer)
                {
                    case 'e':
                        Console.Clear();
                        studentSubject.Add(new StudentSubject
                        { 
                            Student = Student.AddStudent(), 
                            Subject = Subject.AddSubject(), 
                            Grade = StudentSubject.AddGrade()
                        });
                        break;
                    case 'v':
                        Console.Clear();
                        foreach (StudentSubject i in studentSubject)
                        {
                            Student.PrintStudent(i.Student);
                            Subject.PrintSubject(i.Subject);
                            Console.WriteLine(i.Grade);
                        }
                        Console.ReadKey();
                        break;
                    case 'c':
                        Console.Clear();
                        data = ReadFromCSV(@"C:\Users\ilyas\source\repos\StudentyZnamky\studsubj.csv");
                        foreach (var row in data)
                        {
                            studentSubject.Add(new StudentSubject
                            {
                            Student = new Student {StudentID = int.Parse(row[0]), Name = row[1], Lastname = row[2]},
                            Subject = new Subject {SubjectID = int.Parse(row[3]), Name = row[4]},
                            Grade = int.Parse(row[5])
                            });
                        }
                        break;
                    case 'x':
                        Console.Clear();
                        data = ReadFromCSV(@"C:\Users\ilyas\source\repos\StudentyZnamky\subjstud.csv");
                        foreach (var row in data)
                        {
                            studentSubject.Add(new StudentSubject
                            {
                                Student = new Student { StudentID = int.Parse(row[2]), Name = row[3], Lastname = row[4] },
                                Subject = new Subject { SubjectID = int.Parse(row[0]), Name = row[1] },
                                Grade = int.Parse(row[5])
                            });
                        }
                        break;
                    case 's':
                        Console.Clear();
                        int idSt; //variable to get an id
                        bool studentExists = false; //variable to check if id exists
                        Console.Write("Zadejte ID studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out idSt))
                        {
                            Console.WriteLine("Musi to byt cislo");
                            Console.Write("Zadejte ID studenta: ");
                        }
                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == idSt)
                            {
                                Student.PrintStudent(row.Student);
                                studentExists = true;
                                break;
                            }
                        }
                        if (!studentExists)
                        {
                            Console.WriteLine("Student s takovym ID neexistuje");
                            Console.ReadKey();
                            break;
                        }
                        Console.WriteLine();
                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == idSt)
                            {
                                Console.Write("    ");
                                Subject.PrintSubject(row.Subject);
                                Console.WriteLine(row.Grade);
                            }
                        }
                        Console.ReadKey();
                        break;
                    case 'p':
                        Console.Clear();
                        int idSb; //variable to get an id
                        bool subjectExists = false; //variable to check if id exists
                        Console.Write("Zadejte ID predmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out idSb))
                        {
                            Console.WriteLine("Musi to byt cislo");
                            Console.Write("Zadejte ID predmeta: ");
                        }
                        foreach (var row in studentSubject)
                        {
                            if (row.Subject.SubjectID == idSb)
                            {
                                Subject.PrintSubject(row.Subject);
                                subjectExists = true;
                                break;
                            }
                        }
                        if (!subjectExists)
                        {
                            Console.WriteLine("Predmet s takovym ID neexistuje");
                            Console.ReadKey();
                            break;
                        }
                        Console.WriteLine();
                        foreach (var row in studentSubject)
                        {
                            if (row.Subject.SubjectID == idSb)
                            {
                                Console.Write("    ");
                                Student.PrintStudent(row.Student);
                                Console.WriteLine(row.Grade);
                            }
                        }
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
