using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;


namespace WebCrawlerForBachelor
{
    class Program
    {
        static void Main(string[] args)
        {

            //* Works for now *//

            string myDirectory = @"C:\Users\Gytis\Documents\Visual Studio 2017\Projects\WebCrawlerForBachelor\WebCrawlerForBachelor\bin\Debug\Articles\";
            string url = "https://www.delfi.lt/";

            /*var node = new Uri("http://127.0.0.1:9200");
            var settings = new ConnectionSettings(node);
            settings.DefaultIndex("default");

            var client = new ElasticClient(settings);*/

            var visited = new List<string> {};
            var sectionList = new List<string> {"https://www.delfi.lt"};

            var webart = new HtmlWeb();

            
            var htmlDoc = webart.Load(url);
            while (htmlDoc == null)
            {
                htmlDoc = webart.Load(url);
            }


            var linkSections = htmlDoc.DocumentNode.SelectNodes("//li/a/@href");
            foreach (var sec in linkSections)
            {
                sectionList.Add(sec.Attributes["href"].Value);
            }

            for (int i = 0; i < sectionList.Count; i++)
            {
                if (!sectionList[i].Contains("https"))
                {
                    sectionList[i] = "https:" + sectionList[i];
                }
            }

            for (int m = 0; m < sectionList.Count; m++)
            {



                var linksArticle = htmlDoc.DocumentNode.SelectNodes("//h3[contains(@class,'headline-title')]/a/@href");  //Linkai - Straipsniu

                if (linksArticle == null)
                    continue;

                var linksList = new List<string>();

                foreach (var link in linksArticle)
                {
                    if (!link.Attributes["href"].Value.Contains("amp;com") && !link.Attributes["href"].Value.Contains("amp;com") && !link.Attributes["href"].Value.Contains("klausyk") && !link.Attributes["href"].Value.Contains("video"))
                        linksList.Add(link.Attributes["href"].Value);
                }
                if (linksArticle == null)
                    continue;

                for (int x = 0; x < linksList.Count; x++)
                {

                    //htmlDoc.LoadHtml(html);

                    //var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//*[@href]");  //Linkai
                    /*var linksArticle = htmlDoc.DocumentNode.SelectNodes("//h3[contains(@class,'head-title')]/a/@href");  //Linkai - Straipsniu*/

                    /*foreach (var item in visited)
                    {
                        if (linksList.Contains(item))
                            linksList.Remove(item);
                    }*/




                    // Straipsnis
                    string link = linksList[x];

                    if (visited.Contains(link)) // Jeigu jau aplankyta tai neimti;
                        continue;

                    visited.Add(link);

                    var sk = link.Split('/');
                    var subject = "/" + sk[3] + "/" + sk[4] + "/"; // Tema  imti inner
                    htmlDoc = webart.Load(link);

                    while (htmlDoc == null)
                    {
                        htmlDoc = webart.Load(link);
                    }
                    //var author = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'delfi-author-name')]/a"); //
                    var author = htmlDoc.DocumentNode.SelectNodes("//meta[contains(@name, 'author')]/@content"); //


                    //Autorius inner
                    if (author == null)
                    {
                        continue;
                    }

                    var title = htmlDoc.DocumentNode.SelectNodes("//h1[contains(@itemprop, 'headline')]");
                    // Pavadinimas imti inner text
                    var articleText = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/div/div/div/p"); // Straipsnio tekstas inner text
                    if (articleText == null)
                        articleText = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/div/div/p");

                    var articleTextJoin = "";
                    foreach (var i in articleText)
                    {
                        articleTextJoin += " " + i.InnerText;
                    }

                    var artc = new Article()
                    {
                        Subject = subject,
                        Author = author[0].Attributes[1].Value,
                        Title = title[0].InnerText,
                        FullText = articleTextJoin
                    };

                    int fileCount = Directory.GetFiles(myDirectory, "*", SearchOption.TopDirectoryOnly).Length;
                    string article = "article-" + fileCount.ToString() + ".txt";
                    using (StreamWriter sw = new StreamWriter(myDirectory + article))
                    {



                        sw.WriteLine("Tema: " + subject
                        + "\r\nAutorius: " + author[0].Attributes[1].Value + "\r\nPavadinimas: " + title[0].InnerText + "\r\nVisas tekstas: " + articleTextJoin);

                    }

                    System.Threading.Thread.Sleep(4000);
                }
            }

            Console.ReadLine();
        }
    }
}
