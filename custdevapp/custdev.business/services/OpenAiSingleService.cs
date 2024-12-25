using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using custdev.domain.models;
using custdev.domain.interfaces;
using custdev.domain.configuration;
using Microsoft.Extensions.Options;
using custdev.domain.enums;
using System.Net.Http.Headers;

namespace custdev.business.services
{
    public class OpenAiSingleService : IAiService
    {
        private readonly OpenAiDefaultConfiguration _aiConfig;

        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string AudioTranscriptionApiUrl = "https://api.openai.com/v1/audio/transcriptions";

        private const string DefaultModel = "gpt-4o";

        public OpenAiSingleService(
            IOptions<OpenAiDefaultConfiguration> aiConfig
            )
        {
            _aiConfig = aiConfig.Value;
        }

        public OpenAiSingleService(
            OpenAiDefaultConfiguration aiConfig
            )
        {
            _aiConfig = aiConfig;
        }

        public async Task<(PromptData, dynamic)> GetAiResponse(PromptData prompt)
        {
            var response = await GetAiResponseData(prompt);
            return (prompt, response);
        }

        /// <summary>
        /// NEW FUNCTION:
        /// Downloads the audio from the given URL and requests a transcription from OpenAI.
        /// Returns the transcription text on success, or null if the request fails.
        /// </summary>
        /// <param name="audioUrl">The URL to the audio file (e.g., from WhatsApp).</param>
        /// <returns>Transcribed text if successful; otherwise, null.</returns>
        public async Task<string> GetAudioTranscriptionAsync(string audioUrl)
        {
            // Download audio into memory
            byte[] audioData;
            using (var httpClient = new HttpClient())
            {
                // You may need additional error handling here (e.g., 404, etc.)
                audioData = await httpClient.GetByteArrayAsync(audioUrl);
            }

            // Prepare HttpClient for OpenAI call
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _aiConfig.SecretKey);

                // Create the multipart form data
                using (var formContent = new MultipartFormDataContent())
                {
                    // Add audio data
                    var audioFileContent = new ByteArrayContent(audioData);
                    // Adjust the filename and the media type as needed
                    audioFileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mp4");
                    formContent.Add(audioFileContent, "file", "audioFile.mp4");

                    // Required model for transcription
                    formContent.Add(new StringContent("whisper-1"), "model");

                    try
                    {
                        var response = await client.PostAsync(AudioTranscriptionApiUrl, formContent);
                        var result = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            // The transcription result is typically in JSON format:
                            // {
                            //   "text": "Your transcription text."
                            // }
                            dynamic transcriptionJson = JsonConvert.DeserializeObject<dynamic>(result);
                            return transcriptionJson?.text?.ToString();
                        }
                        else
                        {
                            // Handle error scenario
                            // Optionally log the error or throw an exception
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log/handle exception as needed
                        return null;
                    }
                }
            }
        }


        private async Task<dynamic> GetAiResponseData(Prompt prompt)
        {

            (var content, var jsonRequest) = GetRequestContent(prompt);

            bool isSuccessfulCode;
            dynamic parsedResult = null;

            var client = new HttpClient();

            try
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _aiConfig.SecretKey);

                var response = await client.PostAsync(ApiUrl, content);

                string result = await response.Content.ReadAsStringAsync();
                isSuccessfulCode = response.IsSuccessStatusCode;
                if (isSuccessfulCode)
                {
                    parsedResult = JsonConvert.DeserializeObject<dynamic>(result);
                    return parsedResult;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                client.Dispose();
            }

            return parsedResult;
        }

        private (StringContent, string) GetRequestContent(Prompt prompt)
        {
            dynamic data = new ExpandoObject();
            data.model = !String.IsNullOrEmpty(prompt.Model) ? prompt.Model : DefaultModel;

            var messages = new List<dynamic>();
            if (!String.IsNullOrEmpty(prompt.SystemMessage))
                messages.Add(new { role = AiRoles.system, content = prompt.SystemMessage });

            if (!String.IsNullOrEmpty(prompt.UserMessage))
            {
                var userMessage = new ExpandoObject() as dynamic;
                userMessage.role = AiRoles.user;

                if (!String.IsNullOrEmpty(prompt.Image))
                {
                    userMessage.content = new List<dynamic>
                    {
                        new { type = "text", text = prompt.UserMessage },
                        new
                        {
                            type = "image_url",
                            image_url = new { url = prompt.Image }
                        }
                    };
                }
                else
                {
                    userMessage.content = prompt.UserMessage;
                }

                messages.Add(userMessage);
            }

            data.messages = messages;

            if (!String.IsNullOrEmpty(prompt.ResponseFormat))
            {
                if (prompt.ResponseFormat == "json_schema")
                {
                    data.response_format = new
                    {
                        type = prompt.ResponseFormat,
                        json_schema = JsonConvert.DeserializeObject<dynamic>(prompt.JsonForm)
                    };
                }
                else
                {
                    data.response_format = new
                    {
                        type = prompt.ResponseFormat
                    };
                }
            }

            data.temperature = prompt.Temperature;
            data.presence_penalty = 0;
            data.frequency_penalty = 0;

            if (prompt.MaxTokens > 0)
                data.max_tokens = prompt.MaxTokens;

            if (!String.IsNullOrEmpty(prompt.Function))
            {
                var function = JsonConvert.DeserializeObject<dynamic>(prompt.Function);
                data.tools = new[]
                {
                    new
                    {
                        type = "function",
                        function = function
                    }
                };
                data.tool_choice = "required";
            }

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return (content, json);

        }

       
    }
}
