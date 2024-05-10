using System;
using System.IO;
using System.Collections.Generic;
using static StudentyZnamky.Program;
using System.Diagnostics;
using System.Xml;
using System.Threading;
using System.Xml.Serialization;

namespace StudentyZnamky
{
    internal class Program
    {
        public struct Student
        {
            public int StudentID { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public Student(int studentID, string firstname, string lastname)
            {
                StudentID = studentID;
                Firstname = firstname;
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
                Console.Write($"{student.StudentID} {student.Firstname} {student.Lastname} ");
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

        public struct StudentSubject : IComparable<StudentSubject>
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
            public int CompareTo(StudentSubject other)
            {
                int studentIDComparison = this.Student.StudentID.CompareTo(other.Student.StudentID);
                if (studentIDComparison != 0)
                    return studentIDComparison;

                return this.Subject.SubjectID.CompareTo(other.Subject.SubjectID);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            return data;
        }

        static List<StudentSubject> ReadFromXML(string rootPath)
        {
            List<string> tempStudSubj = new List<string>(6);
            List<StudentSubject> studentSubject = new List<StudentSubject>();
            
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(rootPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot != null)
            {
                foreach (XmlElement xnode in xRoot)
                {
                    foreach(XmlNode childnode in xnode.ChildNodes)
                    {
                        switch (childnode.Name)
                        {
                            case "studentid":
                                tempStudSubj.Add(childnode.InnerText);
                                break;
                            case "firstname":
                                tempStudSubj.Add(childnode.InnerText);
                                break;
                            case "lastname":
                                tempStudSubj.Add(childnode.InnerText);
                                break;
                            case "subjectid":
                                tempStudSubj.Add(childnode.InnerText);
                                break;
                            case "subjectname":
                                tempStudSubj.Add(childnode.InnerText);
                                break;
                            case "grade":
                                tempStudSubj.Add(childnode.InnerText);
                                break;
                        }
                    }
                    studentSubject.Add(new StudentSubject
                    {
                        Student = new Student { StudentID = int.Parse(tempStudSubj[0]), Firstname = tempStudSubj[1], Lastname = tempStudSubj[2] },
                        Subject = new Subject { SubjectID = int.Parse(tempStudSubj[3]), Name = tempStudSubj[4] },
                        Grade = int.Parse(tempStudSubj[5])
                    });
                    tempStudSubj.Clear();
                }
            }

            return studentSubject;
        }

        static void Main(string[] args)
        {
            List<StudentSubject> studentSubject = new List<StudentSubject>();
       

            char answer;
            bool runProgram = true;
            int newGrade;
            while (runProgram)
            {
                Console.Clear();
                Console.WriteLine("-------------------");
                Console.WriteLine("Pridat studenta [e]");
                Console.WriteLine("Nacist csv soubor [c]");
                Console.WriteLine("Nacist xml soubor [b]");
                Console.WriteLine("------------------------------");
                Console.WriteLine("Vypsat data [v]");
                Console.WriteLine("Vypsat studenta [s]");
                Console.WriteLine("Vypsat predmet [p]");
                Console.WriteLine("Ulozit data do csv [y]");
                Console.WriteLine("Ulozit data do xml [t]");
                Console.WriteLine("------------------");
                Console.WriteLine("Vymazat jednu znamku studenta [h]");
                Console.WriteLine("Vymazat studenta [j]");
                Console.WriteLine("Vymazat predmet [k]");
                Console.WriteLine("Vymazat data [d]");
                Console.WriteLine("Zmenit znamku studenta [z]");
                Console.WriteLine("To quit [q]");
                Console.WriteLine("-----------");
                Console.Write("Zadejte akci: ");
                answer = char.ToLower(Console.ReadKey().KeyChar);

                switch (answer)
                {
                    case 'e'://pridat studenta z konzole
                        Console.Clear();
                        studentSubject.Add(new StudentSubject
                        { 
                            Student = Student.AddStudent(), 
                            Subject = Subject.AddSubject(), 
                            Grade = StudentSubject.AddGrade()
                        });
                        break;
                    case 'c'://nacist data z csv souboru
                        Console.Clear();
                        Console.Write("Zadejte cestu souboru: ");
                        string cestaCsv = Console.ReadLine();
                        List<string[]> dataCsv = ReadFromCSV(cestaCsv);
                        //C:\Users\ilyas\source\repos\StudentyZnamky\studsubj.csv
                        foreach (var row in dataCsv)
                        {
                            studentSubject.Add(new StudentSubject
                            {
                            Student = new Student {StudentID = int.Parse(row[0]), Firstname = row[1], Lastname = row[2]},
                            Subject = new Subject {SubjectID = int.Parse(row[3]), Name = row[4]},
                            Grade = int.Parse(row[5])
                            });
                        }
                        studentSubject.Sort();
                        break;
                    case 'b'://nacist data z xml souboru
                        Console.Clear();
                        Console.Write("Zadejte cestu souboru: ");
                        string cestaXml = Console.ReadLine();
                        List<StudentSubject> dataXml = ReadFromXML(cestaXml);

                        studentSubject.AddRange(dataXml);
                        studentSubject.Sort();
                        break;
                    case 'v'://vypsat data
                        Console.Clear();
                        foreach (StudentSubject i in studentSubject)
                        {
                            Student.PrintStudent(i.Student);
                            Subject.PrintSubject(i.Subject);
                            Console.WriteLine(i.Grade);
                        }
                        Console.ReadKey();
                        break;
                    case 's'://vypsat znamky studenta
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
                    case 'p'://vypsat znamky studentu za predmet
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
                    case 'y'://ukladani dat do csv
                        int csvFileCount = 1;

                        Console.Clear();
                        string csvFilePath = $"data{csvFileCount}.csv";

                        using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                        {
                            foreach (StudentSubject row in studentSubject)
                            {
                                writer.WriteLine($"{row.Student.StudentID},{row.Student.Firstname},{row.Student.Lastname},{row.Subject.SubjectID},{row.Subject.Name},{row.Grade}");
                            }
                        }
                        csvFileCount++;
                        Console.WriteLine("Data jsou ulozeny " + csvFilePath);
                        Console.ReadKey();

                        break;
                    case 't'://ukladani dat do xml
                        int xmlFileCount = 1;

                        Console.Clear();
                        string xmlFilePath = $"data{xmlFileCount}.xml";

                        XmlWriterSettings set = new XmlWriterSettings();
                        set.Indent = true;

                        using (XmlWriter writer = XmlWriter.Create(xmlFilePath, set))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("studentsubject");

                            foreach (StudentSubject node in studentSubject)
                            {
                                writer.WriteStartElement("student");

                                writer.WriteElementString("studentid", node.Student.StudentID.ToString());
                                writer.WriteElementString("firstname", node.Student.Firstname);
                                writer.WriteElementString("lastname", node.Student.Lastname);
                                writer.WriteElementString("subjectid", node.Subject.SubjectID.ToString());
                                writer.WriteElementString("subjectname", node.Subject.Name);
                                writer.WriteElementString("grade", node.Grade.ToString());

                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                            writer.Flush();
                        }
                        xmlFileCount++;
                        Console.WriteLine("Data jsou ulozeny " + xmlFilePath);
                        Console.ReadKey();

                        break;
                    case 'h'://smazat znamku studenta jednoho predmeta
                        Console.Clear();
                        int tempStudId, tempSubjId;

                        Console.Write("Zadejte id studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out tempStudId))
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id studenta: ");
                        }
                        Console.Write("Zadejte id prerdmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out tempSubjId))
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id predmeta: ");
                        }

                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == tempStudId && row.Subject.SubjectID == tempSubjId)
                            {
                                studentSubject.Remove(row);
                                break;
                            }
                        }
                        break;
                    case 'j'://smazat studenta
                        Console.Clear();
                        Console.Write("Zadejte id studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out tempStudId))
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id studenta: ");
                        }

                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == tempStudId)
                            {
                                studentSubject.Remove(row);
                            }
                        }
                        break;
                    case 'k'://smazat studenta
                        Console.Clear();
                        Console.Write("Zadejte id predmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out tempSubjId))
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id predmeta: ");
                        }

                        foreach (var row in studentSubject)
                        {
                            if (row.Subject.SubjectID == tempSubjId)
                            {
                                studentSubject.Remove(row);
                            }
                        }
                        break;
                    case 'd':
                        Console.Clear();
                        Console.WriteLine("Chcete smazat data? [y]");
                        if (Console.ReadKey().KeyChar == 'y')
                            studentSubject.Clear();
                        break;
                    case 'z'://zmena znamky studenta za predmet
                        Console.Clear();
                        int indexList;
                        Console.Write("Zadejte id studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out tempStudId) || tempStudId < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id studenta: ");
                        }
                        Console.Write("Zadejte id prerdmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out tempSubjId) || tempSubjId < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id predmeta: ");
                        }
                        Console.Write("Zadejte novou znamku: ");
                        while (!int.TryParse(Console.ReadLine(), out newGrade) || newGrade > 4 || newGrade < 1)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte novou znamku: ");
                        }

                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == tempStudId && row.Subject.SubjectID == tempSubjId)
                            {
                                indexList = studentSubject.IndexOf(row);
                                studentSubject[indexList] = new StudentSubject
                                {
                                    Student = new Student { StudentID = row.Student.StudentID, Firstname = row.Student.Firstname, Lastname = row.Student.Lastname },
                                    Subject = new Subject { SubjectID = row.Subject.SubjectID, Name = row.Subject.Name },
                                    Grade = newGrade
                                };
                                break;
                            }
                        }
                        break;
                    case 'q'://ukoncit program
                        runProgram = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}