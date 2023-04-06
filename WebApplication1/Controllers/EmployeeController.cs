using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.Arm;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"Select EmployeeID, EmployeeName, Department,
                            convert(varchar(10),DateOfJoining,120) as DateOfJoining, PhotoFileName
                            from dbo.Employee";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee Emp)
        {
            string query = @"Insert into dbo.Employee 
                            (EmployeeName, Department, DateOfJoining, PhotoFileName) 
                            values (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeName", Emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", Emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", Emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", Emp.PhotoFileName);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(Employee Emp)
        {
            string query = @"Update dbo.Employee 
                            set EmployeeName =@EmployeeName,
                            Department =@Department,
                            DateOfJoining = @DateOfJoining,
                            PhotoFileName = @PhotoFileName
                            where EmployeeID = @EmployeeID";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeID", Emp.EmployeeID);
                    myCommand.Parameters.AddWithValue("@EmployeeName", Emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", Emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", Emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", Emp.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"Delete from dbo.Employee where EmployeeID = @EmployeeID";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeID", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Deleted Successfully");
        }

        [Route("SaveFile")]
        [HttpPost]

        public JsonResult SaveFile()
        {

            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;


                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(fileName);
            }

            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }






    }
}

