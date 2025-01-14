﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Chat.WordGame.WebHelpers;
using FluentAssertions;
using Xunit;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ExceptionExtensions;

namespace PartyGamesTests.WordGame.WebHelpers
{
    public class WebDictionaryRequestHelperTests
    {
        [Fact]
        public void WhenWordDoesNotExistExistenceRequestShouldReturnFalse()
        {
            var url = "https://www.dictionary.com/browse";
            var word = "word-that-doesnt-exist";
            var webRequestHelper = Substitute.For<IWebRequestHelper>();

            webRequestHelper
                .Create($"{url}/{word}")
                .Throws(new Exception());

            var webDictionaryRequestHelper = new WebDictionaryRequestHelper(webRequestHelper);
            var response = webDictionaryRequestHelper.MakeExistenceRequest(word);

            response.Should().BeFalse();
        }

        [Fact]
        public void WhenWordExistsExistenceRequestShouldReturnTrue()
        {
            var url = "https://www.dictionary.com/browse";
            var word = "word-that-doesnt-exist";
            var webRequestHelper = Substitute.For<IWebRequestHelper>();

            webRequestHelper
                .Create($"{url}/{word}")
                .Returns(WebRequest.CreateDefault(new Uri($"{url}/{word}")));
            
            var webDictionaryRequestHelper = new WebDictionaryRequestHelper(webRequestHelper);
            var response = webDictionaryRequestHelper.MakeExistenceRequest(word);

            response.Should().BeTrue();
        }
        
        [Fact]
        public void WhenWordExistsContentRequestShouldReturnAStringContainingTheWord()
        {
            var word = "realWord";
            
            // https://stackoverflow.com/questions/9823039/is-it-possible-to-mock-out-a-net-httpwebresponse
            var responseText = $"blah blah blah {word} blah";
            var bytes = Encoding.UTF8.GetBytes(responseText);
            var responseStream = new MemoryStream();
            responseStream.Write(bytes, 0, bytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var myResponse = Substitute.For<HttpWebResponse>();
            myResponse.GetResponseStream().Returns(responseStream);

            var myRequest = Substitute.For<HttpWebRequest>();
            myRequest.GetResponse().Returns(myResponse);
            
            var webRequestHelper = Substitute.For<IWebRequestHelper>();

            webRequestHelper
                .Create(Arg.Any<string>())
                .Returns(myRequest);
            
            webRequestHelper
                .GetResponse(Arg.Any<WebRequest>())
                .Returns(myResponse);

            var webDictionaryRequestHelper = new WebDictionaryRequestHelper(webRequestHelper);
            var response = webDictionaryRequestHelper.MakeContentRequest(word);

            response.Should().Contain(word);
        }
        
        [Fact]
        public void WhenRequestThrowsExceptionContentRequestShouldReturnEmptyString()
        {
            var url = "https://www.collinsdictionary.com/dictionary/english";
            var word = "not-a-word";
            var webRequestHelper = Substitute.For<IWebRequestHelper>();

            webRequestHelper
                .Create($"{url}/{word}")
                .Throws(new Exception());

            var webDictionaryRequestHelper = new WebDictionaryRequestHelper(webRequestHelper);
            var response = webDictionaryRequestHelper.MakeContentRequest(word);

            response.Should().Be("");
        }
        
        [Fact]
        public void WhenWordDoesNotExistContentRequestShouldNotContainWord()
        {
            var word = "not-a-word";
            
            // https://stackoverflow.com/questions/9823039/is-it-possible-to-mock-out-a-net-httpwebresponse
            var responseText = $"blah blah blah only real words blah";
            var bytes = Encoding.UTF8.GetBytes(responseText);
            var responseStream = new MemoryStream();
            responseStream.Write(bytes, 0, bytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var myResponse = Substitute.For<HttpWebResponse>();
            myResponse.GetResponseStream().Returns(responseStream);

            var myRequest = Substitute.For<HttpWebRequest>();
            myRequest.GetResponse().Returns(myResponse);
            
            var webRequestHelper = Substitute.For<IWebRequestHelper>();

            webRequestHelper
                .Create(Arg.Any<string>())
                .Returns(myRequest);
            
            webRequestHelper
                .GetResponse(Arg.Any<WebRequest>())
                .Returns(myResponse);
            
            var webDictionaryRequestHelper = new WebDictionaryRequestHelper(webRequestHelper);
            var response = webDictionaryRequestHelper.MakeContentRequest(word);

            response.Should().NotContain(word);
        }
    }
}