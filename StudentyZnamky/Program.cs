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
        static int xmlFileCount = 1;
        static int csvFileCount = 1;
        static int studID, subjID;

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

            //Calls the constructor to create a student
            public static Student AddStudent()
            {
                int studentID;
                Console.Write("Zadejte ID studenta: ");
                while (!int.TryParse(Console.ReadLine(), out studentID) || studID < 0)
                {
                    Console.WriteLine("Nezadal jste cislo nebo je spatne");
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

            //Calls the constructor to create a subject
            public static Subject AddSubject()
            {
                int subjectID;
                Console.Write("Zadejte ID predmeta: ");
                while (!int.TryParse(Console.ReadLine(), out subjectID) || studID < 0)
                {
                    Console.WriteLine("Nezadal jste cislo nebo je spatne");
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

            /*
             * Implementation of CompareTo method from IComparable<T> for sorting data
             * New added data needs to be sorted by values
             * First it sortes by student ID and then by a subject ID
             */
            public int CompareTo(StudentSubject other)
            {
                int studentIDComparison = this.Student.StudentID.CompareTo(other.Student.StudentID);
                if (studentIDComparison != 0)
                    return studentIDComparison;

                return this.Subject.SubjectID.CompareTo(other.Subject.SubjectID);
            }
        }

        /*
         * Method reads a CSV file and returns a List of string arrays
         */
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

        /*
         * Method reads an XML file and returns a List of StudentSubject
         * 
         * List<string> tempStudSubj will be storing every data from a root <student>
         * And will be cleared every time a root is finished
         */
        static List<StudentSubject> ReadFromXML(string rootPath)
        {
            List<string> tempStudSubj = new List<string>();
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
                        //Looking for elements and storing data using switch case
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

                    //Creating a new StudentSubject element and adding to the List
                    studentSubject.Add(new StudentSubject
                    {
                        Student = new Student { StudentID = int.Parse(tempStudSubj[0]), Firstname = tempStudSubj[1], Lastname = tempStudSubj[2] },
                        Subject = new Subject { SubjectID = int.Parse(tempStudSubj[3]), Name = tempStudSubj[4] },
                        Grade = int.Parse(tempStudSubj[5])
                    });

                    //List should be cleared after every cicle of reading one root in XML file
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
                    //Add data using the console
                    case 'e':
                        Console.Clear();

                        //Creates a new struct element and adds it to a List
                        studentSubject.Add(new StudentSubject
                        { 
                            Student = Student.AddStudent(), 
                            Subject = Subject.AddSubject(), 
                            Grade = StudentSubject.AddGrade()
                        });

                        //Sorting data by ID
                        studentSubject.Sort();

                        break;

                    //Read data from a CSV file
                    case 'c':
                        Console.Clear();
                        Console.Write("Zadejte cestu souboru: ");
                        string pathCsv = Console.ReadLine();

                        List<string[]> dataCsv = ReadFromCSV(pathCsv);

                        /*
                         * ReadFromCSV method returns List of string arrays
                         * so it creates new StudentSubject elements
                         * by indexing each given string
                         * and calling Constructors of Student and Subject
                         */
                        foreach (var row in dataCsv)
                        {
                            studentSubject.Add(new StudentSubject
                            {
                            Student = new Student {StudentID = int.Parse(row[0]), Firstname = row[1], Lastname = row[2]},
                            Subject = new Subject {SubjectID = int.Parse(row[3]), Name = row[4]},
                            Grade = int.Parse(row[5])
                            });
                        }

                        //Sorting data by ID
                        studentSubject.Sort();
                        break;

                    //Read data from an XML file
                    case 'b':
                        Console.Clear();
                        Console.Write("Zadejte cestu souboru: ");
                        string pathXml = Console.ReadLine();

                        List<StudentSubject> dataXml = ReadFromXML(pathXml);

                        /*
                         * ReadFromXML methos return a List of StudentSubject
                         * and it needs to be joined with other data
                         * by using List.AddRange() here
                        */
                        studentSubject.AddRange(dataXml);

                        //Sorting data by ID
                        studentSubject.Sort();
                        break;

                    //Prints all the data
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

                    //Prints a student and his grades
                    case 's':
                        int idStudent; //Variable to get an ID
                        bool studentExists = false; //Variable to check if ID exists

                        Console.Clear();

                        //Checks if ID is a number and has the right format
                        Console.Write("Zadejte ID studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out idStudent) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte ID studenta: ");
                        }

                        //foreach loop looking for a student ID to know what needs to be printed
                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == idStudent)
                            {
                                Student.PrintStudent(row.Student);
                                studentExists = true;
                                break;
                            }
                        }

                        //Checks if student was found
                        if (!studentExists)
                        {
                            Console.WriteLine("Student s takovym ID neexistuje");
                            Console.ReadKey();
                            break;
                        }

                        //Prints subjects that a student have
                        Console.WriteLine();
                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == idStudent)
                            {
                                Console.Write("    ");
                                Subject.PrintSubject(row.Subject);
                                Console.WriteLine(row.Grade);
                            }
                        }
                        Console.ReadKey();
                        break;

                    //Prints a subject and students grades
                    case 'p':
                        int idSubject; //Variable to get an id
                        bool subjectExists = false; //Variable to check if id exists

                        Console.Clear();

                        //Checks if ID is a number and has the right format
                        Console.Write("Zadejte ID predmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out idSubject) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte ID predmeta: ");
                        }

                        //foreach loop looking for a subject ID to know what needs to be printed
                        foreach (var row in studentSubject)
                        {
                            if (row.Subject.SubjectID == idSubject)
                            {
                                Subject.PrintSubject(row.Subject);
                                subjectExists = true;
                                break;
                            }
                        }

                        //Checks if subject was found
                        if (!subjectExists)
                        {
                            Console.WriteLine("Predmet s takovym ID neexistuje");
                            Console.ReadKey();
                            break;
                        }
                        Console.WriteLine();

                        //Prints students that have that subject
                        foreach (var row in studentSubject)
                        {
                            if (row.Subject.SubjectID == idSubject)
                            {
                                Console.Write("    ");
                                Student.PrintStudent(row.Student);
                                Console.WriteLine(row.Grade);
                            }
                        }
                        Console.ReadKey();
                        break;

                    //Save data to a CSV file
                    case 'y':
                        Console.Clear();

                        //Generates a name of a file
                        string csvFilePath = $"data{csvFileCount}.csv";

                        //Creates a CSV file using StreamWriter and adds data separeting by comma
                        using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                        {
                            foreach (StudentSubject row in studentSubject)
                            {
                                writer.WriteLine($"{row.Student.StudentID},{row.Student.Firstname},{row.Student.Lastname},{row.Subject.SubjectID},{row.Subject.Name},{row.Grade}");
                            }
                        }

                        //Increments a number of saved CSV files during program run
                        csvFileCount++;
                        Console.WriteLine("Data jsou ulozeny: " + csvFilePath);
                        Console.ReadKey();
                        break;

                    //Save data to an XML file
                    case 't':
                        Console.Clear();

                        //Generates a name of a file
                        string xmlFilePath = $"data{xmlFileCount}.xml";

                        XmlWriterSettings set = new XmlWriterSettings();
                        set.Indent = true;

                        /*
                         * Creates a file using XmlWriter
                         * Starts with the element "studentsubject"
                         * Main roots are "student" and they have all needed data inside
                         */
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
                        //Increments a number of saved XML files during program run
                        xmlFileCount++;
                        Console.WriteLine("Data jsou ulozeny " + xmlFilePath);
                        Console.ReadKey();

                        break;

                    //Delete a grade of a student in one subject
                    case 'h':
                        Console.Clear();

                        //Checks if student ID is a number and has the right format
                        Console.Write("Zadejte id studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out studID) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id studenta: ");
                        }

                        //Checks if subject ID is a number and has the right format
                        Console.Write("Zadejte id prerdmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out subjID) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id predmeta: ");
                        }

                        //foreach loop looking for a student and subject that needs to be deleted
                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == studID && row.Subject.SubjectID == subjID)
                            {
                                //Deletes data using List.Remove()
                                studentSubject.Remove(row);
                                break;
                            }
                        }
                        break;

                    //Delete all data of one student
                    case 'j':
                        Console.Clear();

                        //Checks if entered student ID is a number
                        Console.Write("Zadejte id studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out studID) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id studenta: ");
                        }

                        /*
                         * Starts to delete data from the last index of a List
                         * Because using List.Remove() shifts indexes of a List
                         * And it's possible miss something in case of 
                         * Going through a cicle from index 0
                         */
                        for (int i = studentSubject.Count - 1; i >= 0; i--)
                            if (studentSubject[i].Student.StudentID == studID)
                                studentSubject.Remove(studentSubject[i]);

                        break;

                    //Delete all data of a subject
                    case 'k':
                        Console.Clear();

                        //Checks if entered subject ID is a number
                        Console.Write("Zadejte id predmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out subjID) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id predmeta: ");
                        }

                        /*
                         * Starts to delete data from the last index of a List
                         * Because using List.Remove() shifts indexes of a List
                         * And it's possible miss something in case of 
                         * Going through a cicle from index 0
                         */
                        for (int i = studentSubject.Count - 1; i >= 0; i--)
                            if (studentSubject[i].Subject.SubjectID == subjID)
                                studentSubject.Remove(studentSubject[i]);

                        break;

                    //Delete all data
                    case 'd':
                        Console.Clear();

                        //To make sure user wants to delete data
                        Console.WriteLine("Chcete smazat data? [y]");
                        if (Console.ReadKey().KeyChar == 'y')
                            studentSubject.Clear();
                        break;

                    //Change a grade of a student in a subject
                    case 'z':
                        int newGrade, indexList;

                        Console.Clear();

                        //Checks if entered student ID is a number and has the right format
                        Console.Write("Zadejte id studenta: ");
                        while (!int.TryParse(Console.ReadLine(), out studID) || studID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id studenta: ");
                        }

                        //Checks if entered subject ID is a number and has the right format
                        Console.Write("Zadejte id prerdmeta: ");
                        while (!int.TryParse(Console.ReadLine(), out subjID) || subjID < 0)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte id predmeta: ");
                        }

                        //Checks if user entered a grade with the right format (a number in range 1 to 4)
                        Console.Write("Zadejte novou znamku: ");
                        while (!int.TryParse(Console.ReadLine(), out newGrade) || newGrade > 4 || newGrade < 1)
                        {
                            Console.WriteLine("Nezadal jste cislo nebo je spatne");
                            Console.Write("Zadejte novou znamku: ");
                        }

                        /*
                         * In order to change a grade a new StudentSubject element needs to be created
                         * Makes a copy of an element, but with a new grade
                         * List.IndexOf() gets an index of an element that needs to be changed
                         * studentSubject[indexList] replace an element
                         */
                        foreach (var row in studentSubject)
                        {
                            if (row.Student.StudentID == studID && row.Subject.SubjectID == subjID)
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

                    //Finishes the program
                    case 'q':
                        runProgram = false;
                        break;
                    
                    default:
                        break;
                }
            }
        }
    }
}