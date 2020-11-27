using System;
using System.Collections.Generic;
using System.IO; //input output kütüphanesidir..Bilgisayarımız icerisinde dosya işlemleri(modifikasyon okuma) icin kullanılır
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Project.COMMON.Tools
{
    public static class ImageUploader
    {
        //Metodumuz Geriye string deger döndürecek...

        //HttpPostedFileBase => MVC'de bir dosya post edilmek isteniyorsa bu tipteki bir nesne tarafından karsılanır

        //Asagıdaki HTTPostedFileBase tipini cagırabilmek icin System.Web kütüphanesini eklemeyi unutmayın(artık DLL uzantısı gözükmemektedir)
        public static string UploadImage(string serverPath, HttpPostedFileBase file)
        {
            if (file != null)
            {
                Guid uniqueName = Guid.NewGuid();
                //~/Pictures/asdasdasd/uniqueGuid.jpg
                //~ bulundugun projenin kök dizini anlamına gelir
                //MVC'de ~ karakteri normal bir şekilde kök dizine cık olarak @Url.Action kullanmadıgınız sürece algılanmaz. Dolayısıyla bu string karakteri kaldırarak path düzenlememiz gerekir...
                serverPath = serverPath.Replace("~", string.Empty);

                string[] fileArray = file.FileName.Split('.'); //Split metodu size gelen bir string ifadeyi sizin belirlediginiz bir karakterden itibaren elemanlara böler (bilimkurgu.starWars.uzayGemisi.png) ifadesini . ile böldügümüzde karsımıza 3 elemanlı bir dizi cıkar bilimkurgu,starWars,uzayGemisi,png

                string extension = fileArray[fileArray.Length - 1].ToLower(); 

                string fileName = $"{uniqueName}.{extension}";

                if (extension=="jpg"||extension=="gif"||extension=="png"||extension=="jpeg")
                {
                    //Alt taraftaki File System.IO kütüphanesi ile cagrılmaktadır.
                    if (File.Exists(HttpContext.Current.Server.MapPath(serverPath+fileName)))
                    {
                        return "1"; //Ancak Guid kullandıgımız icin aslında aynı ismin tekrarlanmaması konusunda zaten güvendeyiz..Yine de 1'i yani dosya zaten var kodunu burada Algoritmik olarak cıkartmak durumundayız.
                    }
                    else
                    {
                        string filePath = HttpContext.Current.Server.MapPath(serverPath + fileName);
                        file.SaveAs(filePath);
                        return serverPath + fileName;
                    }
                }
                else
                {
                    return "2"; //secilen dosya uzantısı uygun degil
                }


            }
            else
            {
                return "3";//dosya bos
            }
        }


    }
}
