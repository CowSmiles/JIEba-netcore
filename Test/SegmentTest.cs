﻿using JiebaNet.Segmenter;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using jieba.NET;
using Xunit;
using System.Reflection;

namespace Test
{
    public class SegmenterTest
    {
        [Fact]
        public void TestCut()
        {
            var segmenter = new JiebaSegmenter();
            segmenter.LoadUserDict(@"D:\lucene\dict.txt");
            segmenter.LoadUserDictForEmbedded(Assembly.GetCallingAssembly(),"dict.txt");
            var segments = segmenter.Cut("我来到北京清华大学", cutAll: true);
            var resultWords = new List<string> {"我", "来到", "北京", "清华", "清华大学", "华大", "大学"};
            Compared(segments, resultWords);
            
            segments = segmenter.Cut("我来到北京清华大学"); 
            resultWords = new List<string> { "我","来到", "北京", "清华大学"};
            Compared(segments, resultWords);

            segments = segmenter.Cut("他来到了网易杭研大厦");  // 默认为精确模式，同时也使用HMM模型
            resultWords = new List<string> {"他", "来到", "了", "网易", "杭研", "大厦"};
            Compared(segments, resultWords);

            segments = segmenter.CutForSearch("小明硕士毕业于中国科学院计算所，后在日本京都大学深造"); // 搜索引擎模式
            resultWords = new List<string> {"小明","硕士" ,"毕业","于","中国" ,"科学","学院", "科学院" ,"中国科学院","计算", "计算所","，" , "后"
                ,"在" ,"日本","京都" ,"大学", "日本京都大学" ,"深造"};
            Compared(segments, resultWords);

            segments = segmenter.Cut("结过婚的和尚未结过婚的");
            resultWords = new List<string> {"结过婚","的" ,"和" ,"尚未" ,"结过婚","的"};
          
            Compared(segments, resultWords);

            segments = segmenter.Cut("快奔三", false, false);
            resultWords = new List<string> {"快","奔三"};

            Compared(segments, resultWords);
        }

        private void Compared(IEnumerable<string> segments,List<string> resultWords)
        {
            Assert.Equal(segments.Count(),resultWords.Count());
            for (int i = 0; i < segments.Count(); i++)
            {
                Assert.Equal(segments.ElementAt(i),resultWords[i]);
            }
        }

        [Fact]
        public void TestNewCut()
        {
            var segmenter = new JiebaSegmenter();

            var wordInfos = segmenter.Cut2("推荐系统终于发布了最终的版本，点击率蹭蹭上涨");

            Assert.Equal(wordInfos.ElementAt(0).position, 0);
            for (int i = 1; i < wordInfos.Count(); i++)
            {
                Assert.Equal(wordInfos.ElementAt(i).position,
                    wordInfos.ElementAt(i - 1).position + wordInfos.ElementAt(i - 1).value.Length);
            }
        }
        
        [Fact]
        public void TestJIEbaTokenizer()
        {
            var tokenizer = new JieBaTokenizer(TextReader.Null, TokenizerMode.Default);

            Assert.NotEmpty(tokenizer.StopWords);

            Assert.True(tokenizer.StopWords.Contains("是"));
            Assert.True(tokenizer.StopWords.Contains("什么"));
            
        }
    }
}
