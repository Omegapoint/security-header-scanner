namespace headers.security.Scanner;

public interface ICrawler
{
    Task<List<CrawlerResponse>> Crawl(Uri uri, CrawlerConfiguration crawlerConf);
}