
using System.Security.Cryptography;
using System.Text;

namespace PlacementAdmin.services
{
    public class UtilityService
    {
        public string ConvertToBase64(IFormFile profilePic)
        {

            //throw new NotImplementedException();
            using (var memoryStream = new MemoryStream())
            {
                profilePic.CopyTo(memoryStream);
                //TODO remove metaDat
                return Convert.ToBase64String(memoryStream.ToArray());
            }

        }

        public string ComputeHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] byteData = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < byteData.Length; i++)
                {
                    stringBuilder.Append(byteData[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
            //throw new NotImplementedException();
        }
    }
}
