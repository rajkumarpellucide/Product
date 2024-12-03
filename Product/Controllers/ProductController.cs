using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;

namespace Product.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Productdata.json");

        /*[HttpGet(Name = "GetInt")]
        public int Get()
        {
            return 100;
        }*/
        [HttpGet]
        public IActionResult GetData()
        {
            try
            {
                // Load data from the JSON file
                List<DataObject> data = LoadDataFromJson();

                // Return the data
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Handle file read errors or other exceptions
                return StatusCode(500, $"An error occurred while retrieving data: {ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult PostData([FromBody] DataObject data)
        {
            if (string.IsNullOrWhiteSpace(data.ProductName))
            {
                return BadRequest("ProductName cannot be null or empty.");
            }

            // Read existing data from JSON file

            List<DataObject> existingData = LoadDataFromJson();

            // Generate next ID

            data.Id = GetNextId(existingData);

            // Add new data to list
            existingData.Add(data);

            // Write updated data to JSON file
            try
            {
                string updatedJson = JsonSerializer.Serialize(existingData);
                System.IO.File.WriteAllText(_filePath, updatedJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving data: {ex.Message}");
            }
            return Ok("Data saved successfully");
        }
        [HttpPut("{id}")]
        public IActionResult UpdateData(int id, [FromBody] DataObject updatedData)
        {
            if (string.IsNullOrWhiteSpace(updatedData.ProductName))
            {
                return BadRequest("ProductName cannot be null or empty.");
            }

            try
            {
                // Load the existing data from the JSON file
                List<DataObject> existingData = LoadDataFromJson();

                // Find the item to update
                DataObject? existingItem = existingData.FirstOrDefault(d => d.Id == id);

                if (existingItem == null)
                {
                    return NotFound($"Data with Id {id} not found.");
                }

                // Update the item's properties
                existingItem.ProductName = updatedData.ProductName;
                existingItem.ProductType = updatedData.ProductType;
                existingItem.ShopName = updatedData.ShopName;
                existingItem.Location = updatedData.Location;

                // Save the updated data back to the JSON file
                string updatedJson = JsonSerializer.Serialize(existingData);
                System.IO.File.WriteAllText(_filePath, updatedJson);

                return Ok("Data updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating data: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteData(int id)
        {
            try
            {
                List<DataObject> existingData = LoadDataFromJson();
                DataObject? itemToDelete = existingData.FirstOrDefault(d => d.Id == id);

                if (itemToDelete == null)
                {
                    return NotFound($"Data with Id {id} not found.");
                }

                existingData.Remove(itemToDelete);

                string updatedJson = JsonSerializer.Serialize(existingData);
                System.IO.File.WriteAllText(_filePath, updatedJson);

                return Ok($"Data with Id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting data: {ex.Message}");
            }
        }
        private List<DataObject> LoadDataFromJson()

        {

            if (!System.IO.File.Exists(_filePath))
            {
                //using (System.IO.File.Create(_filePath))
                //{
                // File is created even if empty
                //}
                System.IO.File.WriteAllText(_filePath, "[]");

                return new List<DataObject>();
            }

            string jsonData = System.IO.File.ReadAllText(_filePath);

            //if(String.IsNullOrEmpty(jsonData))
            //{
            // return new List<DataObject>();
            //}
            //return System.Text.Json.JsonSerializer.Deserialize<List<DataObject>>(jsonData);

            return string.IsNullOrWhiteSpace(jsonData) ? new List<DataObject>() : JsonSerializer.Deserialize<List<DataObject>>(jsonData);

        }
        private int GetNextId(List<DataObject> data)
        {
            if (!data.Any())

            {

                return 1;

            }
            return data.Max(d => d.Id) + 1;
        }
    }
    public class DataObject

    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductType { get; set; }
        public string? ShopName { get; set; }
        public string? Location { get; set; }
    }
}
