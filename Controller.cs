using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Project1C
{
    public class Controller
    {
        //VARIABLES
        private PDFController _pdfController;
        private List<string> _lstCourseDescriptions;
        private List<string> _lstKeywords;
        private double _totalSimScore;    
        private List<string> _lstStopWords;
        private Dictionary<string, double> _dicPairedSimilarityScores; 
        private Dictionary<string, double> _dicAvgSimilarityScores;
        private Stopwatch _stpWatch;

        ////////////////////////////////////CONSTRUCTOR/////////////////////////////////////////////////
        public Controller()
        {
            _lstStopWords = new List<string>();
            _lstKeywords = new List<string>();
            _lstCourseDescriptions = new List<string>();
            _dicPairedSimilarityScores = new Dictionary<string, double>();
            _dicAvgSimilarityScores = new Dictionary<string, double>();
            _stpWatch = new Stopwatch();

            generateKeywordList();
            generateStopWordList();

            _pdfController = new PDFController(_lstStopWords);
        }

        public void getDigitalTransformationScore(Course course)
        {
            //STEP 1
            _stpWatch.Start();
            setArrayTextFiles();
            getPairedSimilarityScores();
            _stpWatch.Stop();
            Debug.WriteLine("Step 1: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 2 & 3
            _stpWatch.Start();
            getAverageSimilarityScore();
            _stpWatch.Stop();
            Debug.WriteLine("Step 2 & 3: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 4
            _stpWatch.Start();
            course.StrFormattedCourseDesc = formatStringForLooping(course.StrCourseDesc);
            _stpWatch.Stop();
            Debug.WriteLine("Step 4: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 5
            _stpWatch.Start();
            getJaccardSimilarityCoefficient(course);
            _stpWatch.Stop();
            Debug.WriteLine("Step 5: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 6
            _stpWatch.Start();
            getWeightedSimilarityScore(course);
            _stpWatch.Stop();
            Debug.WriteLine("Step 6: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 7
            _stpWatch.Start();
            getLinearCombination(course);
            _stpWatch.Stop();
            Debug.WriteLine("Step 7: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 8 & 9
            _stpWatch.Start();
            computeAverageSimScore(course);
            _stpWatch.Stop();
            Debug.WriteLine("Step 8 & 9: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();

            //STEP 10
            _stpWatch.Start();
            computeCosineScore(course);
            _stpWatch.Stop();
            Debug.WriteLine("Step 10: " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            _stpWatch.Reset();
        }

        ///////////////////////////////////////STEP 1 BLOCK//////////////////////////////////////////////
        public void setArrayTextFiles()
        {
            _pdfController.setArrayTextFiles();
        }
        public void generateKeywordList()   
        {
            _lstKeywords.Add("Digital Transformation");
            _lstKeywords.Add("Digital Technologies");
            _lstKeywords.Add("Digital Data");
            _lstKeywords.Add("Digitization");
            _lstKeywords.Add("Digitalization");
            _lstKeywords.Add("Computerization");
            _lstKeywords.Add("Social Media");
            _lstKeywords.Add("Social Computing");
            _lstKeywords.Add("Mobile Computing");
            _lstKeywords.Add("Mobile Application");
            _lstKeywords.Add("Pervasive Computing");
            _lstKeywords.Add("Mobile Commerce");
            _lstKeywords.Add("Data Analytics");
            _lstKeywords.Add("Business Analytics");
            _lstKeywords.Add("Business Intelligence");
            _lstKeywords.Add("Cloud Computing");
            _lstKeywords.Add("Cloud Service");
            _lstKeywords.Add("Digital Society");
            _lstKeywords.Add("Digital Network");
            _lstKeywords.Add("Digital Service");
            _lstKeywords.Add("Softwaredefined Infrastructure");
            _lstKeywords.Add("Softwaredefined Network");
            _lstKeywords.Add("Agile Software Development");
            _lstKeywords.Add("User Experience");
            _lstKeywords.Add("Dev Ops");
            _lstKeywords.Add("Service Oriented Computing");
            _lstKeywords.Add("Service Oriented Architecture");
            _lstKeywords.Add("Microservice");
            _lstKeywords.Add("Application Programming Interface");
            _lstKeywords.Add("Web Service");
            _lstKeywords.Add("Digital Business");
            _lstKeywords.Add("Digital Business Model");
            _lstKeywords.Add("Digital Innovation");
            _lstKeywords.Add("Digital Product");
            _lstKeywords.Add("Disruptive Innovation");
        }

        public void getPairedSimilarityScores()
        {
            //LOOP THROUGH KEYWORD LIST TO CREATE PAIRED SIMILARITY SCORES
            foreach (string keyword in _lstKeywords)
            {
                foreach (string keyword2 in _lstKeywords)
                {
                    if (keyword == keyword2) //CHECK IF WE'RE ON THE SAME WORD
                    {
                        continue;
                    }
                    else if (_dicPairedSimilarityScores.Keys.Contains(keyword + "_" + keyword2) ||
                        _dicPairedSimilarityScores.Keys.Contains(keyword2 + "_" + keyword)) //CHECK IF PAIRED VALUES ALREADY EXIST
                    {
                        continue;
                    }
                    else
                    {
                        double sim = getSimilarity(keyword, keyword2);
                        _dicPairedSimilarityScores.Add(keyword + "_" + keyword2, sim);
                    }
                }
            }
        }

        public double getSimilarity(string value1, string value2)
        {
            int occurances1 = getNumberOfOccurancesInCorpus(value1);
            int occurances2 = getNumberOfOccurancesInCorpus(value2);
            int n = _pdfController.IntNumOfWordsInCorpus(); //Should be absolute total number of words in corpus
            int totalOccurancesTogether = getNumberOfBothOccurances(value1, value2);
            
            //calculate similarity
            Double dbSimilarity = (Math.Max((Math.Log(occurances2) / Math.Log(2)), (Math.Log(occurances1) / Math.Log(2))) - (Math.Log(totalOccurancesTogether) / Math.Log(2))) /
                            ((Math.Log(n)/Math.Log(2)) - Math.Min((Math.Log(occurances2) / Math.Log(2)), (Math.Log(occurances1) / Math.Log(2))));

            //check if similarity is "NaN" or "Infinity"
            if (Double.IsNaN(dbSimilarity))
            {
                dbSimilarity = 0;               //Find ~2 examples of each case
            }
            else if (Double.IsInfinity(dbSimilarity))
            {
                dbSimilarity = 100; 
            }

            //add to total scores
            _totalSimScore += dbSimilarity; 

            return dbSimilarity;
        }
        //returns how many times a keyword appears in a text
        private int getNumberOfOccurancesInCorpus(string value)
        {
            int occurances = 0;

            foreach (string t in _pdfController.TxtFiles())
            {
                occurances += Regex.Matches(t, value, RegexOptions.IgnoreCase).Count;
            }

            return occurances;
        }

        //find how many papers where both keywords exist
        private int getNumberOfBothOccurances(string word1, string word2)
        {
            int occurances1 = 0;
            int occurances2 = 0;
            int totalOccurances = 0;

            foreach (string text in _pdfController.TxtFiles())
            {
                occurances1 = Regex.Matches(text, word1, RegexOptions.IgnoreCase).Count;
                occurances2 = Regex.Matches(text, word2, RegexOptions.IgnoreCase).Count;

                if (occurances1 != 0 && occurances2 != 0)
                {
                    totalOccurances++;
                }
            }

            return totalOccurances;
        }

        ///////////////////////////////////////STEP 2 & 3 BLOCK////////////////////////////////////////////////
        public void getAverageSimilarityScore()
        {
            double score;

            foreach (string k in _lstKeywords)
            {
                score = getAverageScore(k, _dicPairedSimilarityScores);

                _dicAvgSimilarityScores.Add(k, score);
            }
        }

        public double getAverageScore(string keyword, Dictionary<string, double> dicPairedScores)
        {
            List<double> scores = new List<double>();
            double average = 0;

            //loop through dictionary of similarity scores and extract values that contain our keyword
            foreach (KeyValuePair<string, double> kvp in dicPairedScores)
            {
                if (kvp.Key.Contains(keyword))
                {
                    scores.Add(kvp.Value);
                }   
            }

            //compute weighted average of scores
            if (scores != null)
            {
                double total = 0;

                //add all scores together
                foreach(double d in scores)
                {
                    total += d;
                }
                
                //compute average
                average = total / (scores.Count - 1); 
            }

            return average;
        }
        //Receives total of all similarity scores, size of the list of key words
        private double getWeightedSimilarity(int keyListSize)
        {
            double avgWeightSimilarity = _totalSimScore / (keyListSize - 1);
            return avgWeightSimilarity;
        }
        ///////////////////////////////////////////////STEP 4 BLOCK/////////////////////////////////////
        public void generateStopWordList()
        {
            _lstStopWords.Add("i");
            _lstStopWords.Add("me");
            _lstStopWords.Add("my");
            _lstStopWords.Add("myself");
            _lstStopWords.Add("we");
            _lstStopWords.Add("our");
            _lstStopWords.Add("ours");
            _lstStopWords.Add("ourselves");
            _lstStopWords.Add("you");
            _lstStopWords.Add("yours");
            _lstStopWords.Add("yours");
            _lstStopWords.Add("yourself");
            _lstStopWords.Add("yourselves");
            _lstStopWords.Add("he");
            _lstStopWords.Add("him");
            _lstStopWords.Add("his");
            _lstStopWords.Add("himself");
            _lstStopWords.Add("she");
            _lstStopWords.Add("her");
            _lstStopWords.Add("hers");
            _lstStopWords.Add("herself");
            _lstStopWords.Add("it");
            _lstStopWords.Add("its");
            _lstStopWords.Add("itself");
            _lstStopWords.Add("they");
            _lstStopWords.Add("them");
            _lstStopWords.Add("their");
            _lstStopWords.Add("theirs");
            _lstStopWords.Add("themselves");
            _lstStopWords.Add("what");
            _lstStopWords.Add("which");
            _lstStopWords.Add("who");
            _lstStopWords.Add("whom");
            _lstStopWords.Add("this");
            _lstStopWords.Add("that");
            _lstStopWords.Add("these");
            _lstStopWords.Add("those");
            _lstStopWords.Add("am");
            _lstStopWords.Add("is");
            _lstStopWords.Add("are");
            _lstStopWords.Add("was");
            _lstStopWords.Add("were");
            _lstStopWords.Add("be");
            _lstStopWords.Add("been");
            _lstStopWords.Add("being");
            _lstStopWords.Add("have");
            _lstStopWords.Add("has");
            _lstStopWords.Add("had");
            _lstStopWords.Add("having");
            _lstStopWords.Add("do");
            _lstStopWords.Add("does");
            _lstStopWords.Add("did");
            _lstStopWords.Add("doing");
            _lstStopWords.Add("a");
            _lstStopWords.Add("an");
            _lstStopWords.Add("the");
            _lstStopWords.Add("and");
            _lstStopWords.Add("but");
            _lstStopWords.Add("if");
            _lstStopWords.Add("or");
            _lstStopWords.Add("because");
            _lstStopWords.Add("as");
            _lstStopWords.Add("until");
            _lstStopWords.Add("while");
            _lstStopWords.Add("of");
            _lstStopWords.Add("at");
            _lstStopWords.Add("by");
            _lstStopWords.Add("for");
            _lstStopWords.Add("with");
            _lstStopWords.Add("about");
            _lstStopWords.Add("against");
            _lstStopWords.Add("between");
            _lstStopWords.Add("into");
            _lstStopWords.Add("through");
            _lstStopWords.Add("during");
            _lstStopWords.Add("before");
            _lstStopWords.Add("after");
            _lstStopWords.Add("above");
            _lstStopWords.Add("below");
            _lstStopWords.Add("to");
            _lstStopWords.Add("from");
            _lstStopWords.Add("up");
            _lstStopWords.Add("down");
            _lstStopWords.Add("in");
            _lstStopWords.Add("out");
            _lstStopWords.Add("on");
            _lstStopWords.Add("off");
            _lstStopWords.Add("over");
            _lstStopWords.Add("under");
            _lstStopWords.Add("again");
            _lstStopWords.Add("further");
            _lstStopWords.Add("then");
            _lstStopWords.Add("once");
            _lstStopWords.Add("here");
            _lstStopWords.Add("there");
            _lstStopWords.Add("when");
            _lstStopWords.Add("where");
            _lstStopWords.Add("why");
            _lstStopWords.Add("how");
            _lstStopWords.Add("all");
            _lstStopWords.Add("any");
            _lstStopWords.Add("both");
            _lstStopWords.Add("each");
            _lstStopWords.Add("few");
            _lstStopWords.Add("more");
            _lstStopWords.Add("most");
            _lstStopWords.Add("other");
            _lstStopWords.Add("some");
            _lstStopWords.Add("such");
            _lstStopWords.Add("no");
            _lstStopWords.Add("nor");
            _lstStopWords.Add("not");
            _lstStopWords.Add("only");
            _lstStopWords.Add("own");
            _lstStopWords.Add("same");
            _lstStopWords.Add("so");
            _lstStopWords.Add("than");
            _lstStopWords.Add("too");
            _lstStopWords.Add("very");
            _lstStopWords.Add("s");
            _lstStopWords.Add("t");
            _lstStopWords.Add("can");
            _lstStopWords.Add("will");
            _lstStopWords.Add("just");
            _lstStopWords.Add("don");
            _lstStopWords.Add("should");
            _lstStopWords.Add("now");
        }

        public void getCourseDescriptions(List<Course> lstCourses)
        {
            //Excel.Application xlApp = new Excel.Application();
            //Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\IS Core Data - description.xlsx");
            //Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[2];
            //Excel.Range xlRange = xlWorksheet.UsedRange;

            //int rowCount = xlRange.Rows.Count;
            //int colCount = xlRange.Columns.Count;

            //for (int i = 2; i <= 2944; i++)
            //{

            //    string row = Convert.ToString(xlRange.Cells[i, 1].Value2);
            //    List<string> lstInfo = new List<string>();

            //    for (int j = 1; j <= 11; j++)
            //    {
            //        string value = Convert.ToString(xlRange.Cells[i, j].Value2);
            //        if (value == null) value = "NULL";
            //        lstInfo.Add(value);
            //    }

            //    Course course = new Course();
            //    course.StrUniversity = lstInfo[0];
            //    course.StrCollege = lstInfo[1];
            //    course.StrLink = lstInfo[2];
            //    course.StrState = lstInfo[3];
            //    course.StrDept = lstInfo[4];
            //    course.StrTrack = lstInfo[5];
            //    course.StrCourseID = lstInfo[6];
            //    course.StrCourseName = lstInfo[7];
            //    course.StrCourseDesc = lstInfo[8];
            //    course.StrCore = lstInfo[9];
            //    course.StrPrimaryID = lstInfo[10];

            //    lstCourses.Add(course);

            //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            //    {
            //        using (SqlCommand cmd = new SqlCommand())
            //        {
            //            string sql = "INSERT INTO Course(PrimaryID, University, College, Link, State, Department, Track, CourseID, CourseName, CourseDescription, Core)" +
            //            " VALUES(@primaryID, @university, @college, @link, @state, @department, @track, @courseID, @coursename, @coursedesc, @core)";

            //            cmd.Parameters.Add("@primaryID", SqlDbType.NVarChar).Value = course.StrPrimaryID;
            //            cmd.Parameters.Add("@university", SqlDbType.NVarChar).Value = course.StrUniversity;
            //            cmd.Parameters.Add("@college", SqlDbType.NVarChar).Value = course.StrCollege;
            //            cmd.Parameters.Add("@link", SqlDbType.NVarChar).Value = course.StrLink;
            //            cmd.Parameters.Add("@state", SqlDbType.NVarChar).Value = course.StrState;
            //            cmd.Parameters.Add("@department", SqlDbType.NVarChar).Value = course.StrDept;
            //            cmd.Parameters.Add("@track", SqlDbType.NVarChar).Value = course.StrTrack;
            //            cmd.Parameters.Add("@courseID", SqlDbType.NVarChar).Value = course.StrCourseID;
            //            cmd.Parameters.Add("@coursename", SqlDbType.NVarChar).Value = course.StrCourseName;
            //            cmd.Parameters.Add("@coursedesc", SqlDbType.NVarChar).Value = course.StrCourseDesc;
            //            cmd.Parameters.Add("@core", SqlDbType.NVarChar).Value = course.StrCore;

            //            cmd.CommandType = CommandType.Text;
            //            cmd.CommandText = sql;
            //            cmd.Connection = conn;
            //            conn.Open();
            //            int rowsAffected = cmd.ExecuteNonQuery();
            //            conn.Close();
            //        }
            //    }
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "SELECT * FROM COURSE";

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Course course = new Course();
                        course.StrUniversity = dr[1].ToString();
                        course.StrCollege = dr[2].ToString();
                        course.StrLink = dr[3].ToString();
                        course.StrState = dr[4].ToString();
                        course.StrDept = dr[5].ToString();
                        course.StrTrack = dr[6].ToString();
                        course.StrCourseID = dr[7].ToString();
                        course.StrCourseName = dr[8].ToString();
                        course.StrCourseDesc = dr[9].ToString();
                        course.StrCore = dr[10].ToString();
                        course.StrPrimaryID = dr[0].ToString();
                        course.StrFormattedCourseDesc = formatStringForLooping(dr[9].ToString());

                        lstCourses.Add(course);
                    }

                    dr.Close();
                    conn.Close();
                }
            }
        }

        public void removeStopWords(List<String> lstStopWords,List<Course> lstCourses)
        {
            foreach (Course course in lstCourses)
            {
                foreach(string stopword in lstStopWords)
                {
                    course.StrCourseDesc = Regex.Replace(course.StrCourseDesc, @"\b" + stopword + @"\b", "");
                }
            }
        }
        
        ////////////////////////////////////STEP 5//////////////////////////////////////////////////////////////
        public void getJaccardSimilarityCoefficient(Course course)
        {
            //foreach (Course course in lstCourses)
            //{
                string description = course.StrFormattedCourseDesc;

                //foreach (string keyword in _lstKeywords)
                //{
                    int shared = getNumSharedWords(description);
                    int intLength1 = Regex.Matches(course.StrCourseDesc, "\\w").Count;
                    int intLength2 = _lstKeywords.Count;

                    double dbJaccSim = shared / (intLength1 + intLength2);

                    //check if similarity is "NaN" or "Infinity"
                    if (Double.IsNaN(dbJaccSim)) dbJaccSim = 0;
                    else if (Double.IsInfinity(dbJaccSim)) dbJaccSim = 100; 
                    
                    //add to dictionary
                    course.DbJaccSimScore = dbJaccSim;
                //}
            //}
        }

        public int getNumSharedWords(string description)
        {
            int intShared = 0;

            foreach (string keyword in _lstKeywords)
            {
                intShared += Regex.Matches(description, keyword, RegexOptions.IgnoreCase).Count;
            }

            return intShared;
        }

        ////////////////////////////////////STEP 6//////////////////////////////////////////////////////////////
        public void getWeightedSimilarityScore(Course course)
        {
            int i = 0;

            //foreach(Course course in lstCourses)
            //{
                _stpWatch.Start();
                i++;

                string description = course.StrFormattedCourseDesc;

                //split up each word
                List<string> words = description.Split(' ').ToList();

                //remove empties
                words.RemoveAll(w => string.IsNullOrEmpty(w));

                //for each word in course descrition
                foreach(string word in words)
                {
                    //for each keyword
                    foreach (string keyword in _lstKeywords)
                    {
                        int fW = getNumberOfOccurancesInCorpus(word); //occurances in corpus for description word
                        int fK = getNumberOfOccurancesInCorpus(keyword); //occurances in corpus for keyword
                        int totalOccurancesTogether = getNumberOfBothOccurances(word, keyword);
                        int n = _pdfController.IntNumOfWordsInCorpus();

                        double score = (Math.Max((Math.Log(fW) / Math.Log(2)), (Math.Log(fK) / Math.Log(2))) - (Math.Log(totalOccurancesTogether) / Math.Log(2))) /
                            ((Math.Log(n) / Math.Log(2)) - Math.Min((Math.Log(fW) / Math.Log(2)), (Math.Log(fK) / Math.Log(2))));

                        //check if similarity is "NaN" or "Infinity"
                        if (Double.IsNaN(score)) score = 0;
                        else if (Double.IsInfinity(score)) score = 100; 

                        //if key doesn't exists in dictionary, add it
                        string key = word + "_" + keyword;
                        if (!course.GetDicWeightedSimScore.Keys.Contains(key)) course.GetDicWeightedSimScore.Add(key, score);
                    }
                }
                //course timer
                _stpWatch.Stop();
                Debug.WriteLine("course " + i + ": " + _stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
                _stpWatch.Reset();
            //}
        }
        ////////////////////////////////////STEP 7//////////////////////////////////////////////////////////////
        public void getLinearCombination(Course course)
        {
            //foreach (Course course in lstCourses)
            //{
                string description = course.StrFormattedCourseDesc;

                //split up each word
                List<string> words = description.Split(' ').ToList();

                //remove empties
                words.RemoveAll(w => string.IsNullOrEmpty(w));

                foreach (string word in words)
                {
                    foreach (string keyword in _lstKeywords)
                    {
                        double simJac = course.DbJaccSimScore;
                        double simWeight = course.GetDicWeightedSimScore[word + "_" + keyword];
                        //double positive = Double.PositiveInfinity;
                        //double negative = double.NegativeInfinity;

                        double simLinear = (0.5 * simJac) + (0.5 * simWeight);

                        string key = word + "_" + keyword;
                        if (!course.GetLinearCombScores.Keys.Contains(key)) course.GetLinearCombScores.Add(key, simLinear);
                    }
                }
            //}
        }
        ////////////////////////////////////STEP 89//////////////////////////////////////////////////////////////
        public void computeAverageSimScore(Course course)
        {
            //foreach (Course course in lstCourses)
            //{
                foreach (string keyword in _lstKeywords)
                {
                    List<double> scores = new List<double>();
                    List<double> tempScores = new List<double>();

                    tempScores = (List<double>)course.GetLinearCombScores.Where(k => k.Key.Contains(keyword)).Select(k=>k.Value).ToList();

                    double total = 0; 

                    if (tempScores.Count > 1)
                    {
                        foreach (double score in tempScores)
                        {
                            total += score;
                        }
                    }
                    else if (tempScores.Count == 1)
                    {
                        total = tempScores[0];
                    }

                    int length = Regex.Matches(course.StrCourseDesc, "\\w").Count;
                    double avg = total / tempScores.Count;

                    double avgScore = avg / length;

                    course.GetAvgLinearCombScores.Add(keyword, avgScore);
                }
            //}
        }
        ////////////////////////////////////STEP 10//////////////////////////////////////////////////////////////
        public void computeCosineScore(Course course)
        {
            double vK = 0; //step 23
            double vK2 = 0;

            double vT = 0; //step 89
            double vT2 = 0;

            foreach (double d in _dicAvgSimilarityScores.Values)
            {
                vK += d;
                vK2 += (d * d);
            }

            foreach (double d in course.GetAvgLinearCombScores.Values)
            {
                vT += d;
                vT2 += (d * d);
            }

            course.DbCosineScore = (vK * vT) / (Math.Sqrt(vK2) * Math.Sqrt(vT2));
        }


        ///////////////////////////////////////GENERAL HELPERS///////////////////////////////////////////////////
        private string formatStringForLooping(string text)
        {
            //1. replace punctuation with spaces
            text = Regex.Replace(text, @"[^\w\s]", " ");

            //2. remove stop words
            foreach (string sw in _lstStopWords)
            {
                text = Regex.Replace(text, @"\b" + sw + @"\b", "", RegexOptions.IgnoreCase);
                //text = text.Replace(sw, "");
            }

            //3. remove special characters
            text = Regex.Replace(text, "[^0-9A-Za-z ,]", "");

            //4. remove any numbers
            text = Regex.Replace(text, @"[\d-]", "");

            //5. reduce any amount of spaces to just one
            text = Regex.Replace(text, "[ ]{2,}", " ", RegexOptions.None);

            return text;
        }

        ///////////////////////////////////////GETTER///////////////////////////////////////////////////
        public double TotalSimScore()
        {
            return _totalSimScore;
        }
    }
}