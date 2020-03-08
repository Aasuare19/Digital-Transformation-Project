using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project1C
{
    public class Course
    {
        private string _strUniversity;
        private string _strCollege;
        private string _strLink;
        private string _strState;
        private string _strDept;
        private string _strTrack;
        private string _strCourseID;
        private string _strCourseName;
        private string _strCourseDesc;
        private string _strCore;
        private string _strPrimaryID;
        private string _strFormattedCourseDesc;
        private double _dbJaccSimScore;
        private double _dbCosineScore;
        private Dictionary<string, double> _dicWeightSimScore;
        private Dictionary<string, double> _dicJaccardSimScore;
        private Dictionary<string, double> _dicLinearCombScore;
        private Dictionary<string, double> _dicAvgLinCombScore;

        public Course()
        {
            _dicWeightSimScore = new Dictionary<string, double>();
            _dicJaccardSimScore = new Dictionary<string, double>();
            _dicLinearCombScore = new Dictionary<string, double>();
            _dicAvgLinCombScore = new Dictionary<string, double>();
        }

        public string StrUniversity
        {
            get { return _strUniversity; }
            set { _strUniversity = value; }
        }

        public string StrCollege
        {
            get { return _strCollege; }
            set { _strCollege = value; }
        }

        public string StrLink
        {
            get { return _strLink; }
            set { _strLink = value; }
        }

        public string StrState
        {
            get { return _strState; }
            set { _strState = value; }
        }

        public string StrDept
        {
            get { return _strDept; }
            set { _strDept = value; }
        }

        public string StrTrack
        {
            get { return _strTrack; }
            set { _strTrack = value; }
        }

        public string StrCourseID
        {
            get { return _strCourseID; }
            set { _strCourseID = value; }
        }

        public string StrCourseName
        {
            get { return _strCourseName; }
            set { _strCourseName = value; }
        }

        public string StrCourseDesc
        {
            get { return _strCourseDesc; }
            set { _strCourseDesc = value; }
        }

        public string StrCore
        {
            get { return _strCore; }
            set { _strCore = value; }
        }

        public string StrPrimaryID
        {
            get { return _strPrimaryID; }
            set { _strPrimaryID = value; }
        }

        public double DbJaccSimScore
        {
            get { return _dbJaccSimScore; }
            set { _dbJaccSimScore = value; }
        }
        public double DbCosineScore
        {
            get { return _dbCosineScore; }
            set { _dbCosineScore = value; }
        }
        public Dictionary<string, double> GetDicWeightedSimScore
        {
            get { return _dicWeightSimScore; }
        }

        public string StrFormattedCourseDesc
        {
            get { return _strFormattedCourseDesc; }
            set { _strFormattedCourseDesc = value; }
        }
        public Dictionary<string, double> GetDicJaccardSimScore
        {
            get { return _dicJaccardSimScore; }
        }
        public Dictionary<string, double> GetLinearCombScores
        {
            get { return _dicLinearCombScore; }
        }
        public Dictionary<string, double> GetAvgLinearCombScores
        {
            get { return _dicAvgLinCombScore; }
        }
    }
}