using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Project1C
{
    public partial class _default : System.Web.UI.Page
    {

         protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //POPULATE COURSE TABLE
                DataTable dt = new DataTable();

                String connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();

                string sql = "SELECT CourseID, CourseName, University, State FROM COURSE";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand = cmd;
                da.Fill(dt);

                tblCourse.DataSource = dt;
                tblCourse.DataBind();

                //POPULATE STATE DROPDOWN
                sql = "SELECT DISTINCT State FROM Course ORDER BY State ASC";
                cmd = new SqlCommand(sql, conn);
                da = new SqlDataAdapter(sql, conn);
                da.SelectCommand = cmd;

                dt.Clear();
                da.Fill(dt);
                StateSelect.DataSource = dt;
                StateSelect.DataTextField = "State";
                StateSelect.DataValueField = "State";
                StateSelect.DataBind();

                //POPULATE SCHOOL DROPDOWN
                sql = "SELECT DISTINCT University FROM Course ORDER BY University ASC";
                cmd = new SqlCommand(sql, conn);
                da = new SqlDataAdapter(sql, conn);
                da.SelectCommand = cmd;

                dt.Clear();
                da.Fill(dt);
                SchoolSelect.DataSource = dt;
                SchoolSelect.DataTextField = "University";
                SchoolSelect.DataValueField = "University";
                SchoolSelect.DataBind();

                conn.Close();
            }
            
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            Stopwatch stpWatch = new Stopwatch();
            stpWatch.Start();

            Course course = new Course();
            LinkButton btn = (LinkButton)(sender);
            course.StrCourseID = btn.CommandArgument;

            String connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            string sql = "SELECT * FROM COURSE WHERE CourseID = @courseid";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@courseid", course.StrCourseID);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                dr.Read();
                course.StrUniversity = dr.GetString(1);
                course.StrCollege = dr.GetString(2);
                course.StrLink = dr.GetString(3);
                course.StrState = dr.GetString(4);
                course.StrDept = dr.GetString(5);
                course.StrTrack = dr.GetString(6);
                course.StrCourseName = dr.GetString(8);
                course.StrCourseDesc = dr.GetString(9);
                course.StrCore = dr.GetString(10);
            }

            dr.Close();
            conn.Close();

            Debug.WriteLine("Finished initial course query: " + stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
            Debug.WriteLine("Starting Step 1...");

            Controller controller = new Controller();
            controller.getDigitalTransformationScore(course);


            lblCourseNumName.InnerHtml = course.StrCourseID + " - " + course.StrCourseName;
            lblUniversity.InnerHtml = course.StrUniversity + ", " + course.StrState;
            lblCollege.InnerHtml = course.StrCollege;
            lblDept.InnerHtml = course.StrDept;
            lblTrack.InnerHtml = course.StrTrack;
            lblCore.InnerHtml = "Core: " + course.StrCore;
            lblDescription.InnerHtml = course.StrCourseDesc;
            lblLink.InnerHtml = "<a href=\"" + course.StrLink + "\">" + course.StrLink + "</a>";
            lblScore.InnerHtml = "Digital Transformation Score: " + Math.Round(course.DbCosineScore, 2);

            stpWatch.Stop();
            Debug.WriteLine("Total time: " + stpWatch.Elapsed.ToString(@"h\:mm\:ss"));
        }

        protected void StateSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string state = StateSelect.SelectedValue.ToString();

            //POPULATE COURSE TABLE
            DataTable dt = new DataTable();

            String connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            string sql = "SELECT CourseID, CourseName, University, State FROM COURSE";

            if (!state.Equals("All States")) sql += " WHERE State = @state";

            SqlCommand cmd = new SqlCommand(sql, conn);

            if (!state.Equals("All States")) cmd.Parameters.AddWithValue("@state", state);

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand = cmd;
            da.Fill(dt);

            tblCourse.DataSource = dt;
            tblCourse.DataBind();


            //POULATE SCHOOL DROPDOWN
            SchoolSelect.Items.Clear();
            SchoolSelect.Items.Add("All Schools");

            sql = "SELECT DISTINCT University FROM Course";

            if (!state.Equals("All States")) sql += " WHERE State = @state ORDER BY University ASC";

            cmd = new SqlCommand(sql, conn);

            if (!state.Equals("All States")) cmd.Parameters.AddWithValue("@state", state);

            da = new SqlDataAdapter(sql, conn);
            da.SelectCommand = cmd;

            dt.Clear();
            da.Fill(dt);
            SchoolSelect.DataSource = dt;
            SchoolSelect.DataTextField = "University";
            SchoolSelect.DataValueField = "University";
            SchoolSelect.DataBind();

            SchoolSelect.SelectedValue = "All Schools";

            conn.Close();
        }

        protected void SchoolSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAllStates = false;
            bool isAllSchools = false;
            bool isStateParam = false;
            bool isSchoolParam = false;

            string state = StateSelect.SelectedValue.ToString();
            if (state.Equals("All States")) isAllStates = true;

            string university = SchoolSelect.SelectedValue.ToString();
            if (university.Equals("All Schools")) isAllSchools = true;

            //POPULATE COURSE TABLE
            DataTable dt = new DataTable();

            String connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            string sql = "SELECT CourseID, CourseName, University, State FROM COURSE";

            if (!isAllStates)
            {
                sql += " WHERE State = @state";
                isStateParam = true;
            }

            if (!isAllSchools && isAllStates)
            {
                sql += " WHERE University = @university";
                isSchoolParam = true;
            }
            else if (!isAllSchools && !isAllSchools)
            {
                sql += " AND University = @university";
                isSchoolParam = true;
            }

            SqlCommand cmd = new SqlCommand(sql, conn);

            if (isStateParam) cmd.Parameters.AddWithValue("@state", state);
            if (isSchoolParam) cmd.Parameters.AddWithValue("@university", university);

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand = cmd;
            da.Fill(dt);

            tblCourse.DataSource = dt;
            tblCourse.DataBind();

            conn.Close();
        }
    }
}