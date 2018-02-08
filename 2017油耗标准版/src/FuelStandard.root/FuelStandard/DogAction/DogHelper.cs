using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SuperDog;


namespace DogAction
{
    public class DogHelper
    {
        public const string scope = "<dogscope />";
        public const int FileId = 0xfff4;    // file id of default read/write file
        public DogHelper()
        { }

        public static string ReadData()
        {

            DogFeature feature = DogFeature.Default;
            using (Dog dog = new Dog(feature))
            {
                DogStatus status = dog.Login(VendorCode.Code, scope);
                if ((null == dog) || !dog.IsLoggedIn())
                    return "请插入加密狗";
                DogFile file = dog.GetFile(FileId);
                if (!file.IsLoggedIn())
                {
                    // Not logged into a dog - nothing left to do.  
                    return "加载数据失败";
                }
                int size = 0;
                status = file.FileSize(ref size);
                if (DogStatus.StatusOk != status)
                {
                    return "加载数据失败";
                }

                // read the contents of the file into a buffer
                byte[] bytes = new byte[size];
                status = file.Read(bytes, 0, bytes.Length);
                if (DogStatus.StatusOk != status)
                {
                    return "失败";
                }
                return  System.Text.Encoding.UTF8.GetString(bytes);
            }
        }

    }
}
