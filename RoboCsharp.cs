using Awm.Automacao.Infra.Robos.scrapeScrenn.RoboBase;
using AWM.Automacao.Domain.AggregateObj.ArticliesAgg;
using AWM.Automacao.Domain.Contracts.RoboContract;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Awm.Automacao.Infra.Robos.scrapeScrenn.Articles
{
    public class RoboCsharp : Robo, IRobo
    {
        public RoboCsharp()
        {
            RoboWebClient = new RoboWebClient();
        }

       
        public IEnumerable<object> CarregaPosts()
        {
            NameValueCollection parametros = new NameValueCollection();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //Carrega a página inicial do Blog.
            //Estou atribuindo o resultado ao HtmlAgilityPack para fazer o parse do HTML.
            this.RoboWebClient._allowAutoRedirect = false;
            var ret = this.HttpGet(@"http://netcoders.com.br/");

            var qtdPaginas = HtmlAgilityPack.HtmlEntity.DeEntitize(ConvertUTF(ret.DocumentNode.DescendantsAndSelf().FirstOrDefault(d => d.Attributes["class"] != null && d.Attributes["class"].Value == "wp-pagenavi iegradient").InnerHtml));
            var link = ret.DocumentNode.SelectNodes("//link[@href]").LastOrDefault();
            var href = link.Attributes["href"].Value;
            var h = ret.DocumentNode.SelectSingleNode("//a").Attributes["href"].Value.ToList();



            //Capturando apenas as tags que estão definidas como article e ordenando pelo ID de cada Tag.
            var artigosOrdenados = ret.DocumentNode.Descendants().Where(n => n.Name == "article").OrderBy(d => d.Id).ToList();
            List<Article> artigos = new List<Article>();

            //Percorrendo os artigos que ja foram selecionados.
            foreach (var item in artigosOrdenados)
            {
                var art = new Article();                
                //Carregando o Html de cada artigo.
                doc.LoadHtml(item.InnerHtml);

                //Estou utilizando o HtmlAgilityPack.HtmlEntity.DeEntitize para fazer o HtmlDecode dos textos capturados de cada artigo.
                // Utilizo também o UTF8 para limpar o restante dos Encodes que estiverem na página.
                art.Titulo = HtmlAgilityPack.HtmlEntity.DeEntitize(ConvertUTF(doc.DocumentNode.DescendantsAndSelf().FirstOrDefault(d => d.Attributes["class"] != null && d.Attributes["class"].Value == "post-title entry-title").InnerText));
                art.Data = Convert.ToDateTime(HtmlAgilityPack.HtmlEntity.DeEntitize(doc.DocumentNode.DescendantsAndSelf().FirstOrDefault(d => d.Name == "span" && d.Attributes["class"].Value == "post-time").InnerText));
                art.Descricao = HtmlAgilityPack.HtmlEntity.DeEntitize(ConvertUTF(doc.DocumentNode.DescendantsAndSelf().FirstOrDefault(d => d.Attributes["class"] != null && d.Attributes["class"].Value == "entry-content").InnerText));
                art.Autor = HtmlAgilityPack.HtmlEntity.DeEntitize(ConvertUTF(doc.DocumentNode.DescendantsAndSelf().FirstOrDefault(d => d.Attributes["class"] != null && d.Attributes["class"].Value == "post-author").InnerText));
                art.LnkArtigo = doc.DocumentNode.SelectSingleNode("//a").Attributes["href"].Value;
                
                var txtArtigo = this.HttpGet(art.LnkArtigo);
                

                foreach (var item1 in txtArtigo.DocumentNode.Descendants().Where(x => x.Id == "post-entry").ToList())
                {
                    doc.LoadHtml(item1.InnerHtml);
                    art.TxtArtigo = HtmlAgilityPack.HtmlEntity.DeEntitize(ConvertUTF(doc.DocumentNode.DescendantsAndSelf().FirstOrDefault(d => d.Attributes["class"] != null && d.Attributes["class"].Value == "post-content" && d.Attributes["class"].Value != "cab-author-inner").InnerText));

                }
                yield return art;
            }
        }



        string IRobo.CarregaTexto(string url)
        {
            throw new NotImplementedException();
        }
    }
}

