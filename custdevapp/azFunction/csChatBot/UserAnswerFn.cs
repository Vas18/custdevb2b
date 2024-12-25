using custdev.business.services;
using custdev.domain.db;
using custdev.domain.interfaces;
using custdev.domain.interfaces.db;
using custdev.domain.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace csChatBot
{
    public class UserAnswerFn
    {
        private readonly ILogger<UserAnswerFn> _logger;
        private readonly IAiService _aiService;
        private readonly IDbMessageService _dbMessageService;

        public UserAnswerFn(
            IAiService aiService,
            IDbMessageService dbMessageService,
            ILogger<UserAnswerFn> logger)
        {
            _logger = logger;
            _aiService = aiService;
            _dbMessageService = dbMessageService;
        }

        [Function("answer")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userResponse = JsonConvert.DeserializeObject<UserMsgResponse>(requestBody);

                var dbAdded = await AddToDatabase(userResponse);

                if (!userResponse.IsFinal)
                {
                    return new OkObjectResult(dbAdded);
                }
                else
                {
                    //todo: start report generation
                    return new OkObjectResult(dbAdded);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error processing user response");
                return new BadRequestResult();
            }
        }

        private async Task<bool> AddToDatabase(UserMsgResponse userMsg)
        {
            if (!String.IsNullOrEmpty(userMsg.FileUrl))
            {
                var transcript = await _aiService.GetAudioTranscriptionAsync(userMsg.FileUrl);
                if (!String.IsNullOrEmpty(transcript))
                {
                    userMsg.Text = transcript;
                }
                else
                {
                    userMsg.Text = $"Unable to transcript";
                }
            }

            //add to database
            var dbMsg = Map(userMsg);
            var dbAdded = await _dbMessageService.AddDbMessage(dbMsg);
            return dbAdded;
        }

        private DbMessage Map(UserMsgResponse model)
        {
            var result = new DbMessage
            {
                Id = $"{model.ClientId}-{model.PlatformId}-{StringUtils.GenerateId(length:10,usePrefix:false,upperCase:true)}",
                RequestId = model.PlatformId,
                Language = model.Language,
                Question = model.Question,
                Text = model.Text,
                ClientId = model.ClientId,
                PlatformId = model.PlatformId,
                IsFinal = model.IsFinal,
                DateCreated = DateTime.UtcNow
            };
            return result;
        }

        private async Task CreateReport()
        {
            //json:
            var jsonSchema = "{ \"name\": \"Interview Report Schema\", \"description\": \"Schema for the interview report based on the procurement questionnaire\", \"strict\": true, \"schema\": { \"type\": \"object\", \"properties\": { \"companyInfo\": { \"type\": \"object\", \"description\": \"Информация о компании, в которой работает респондент\", \"properties\": { \"companyName\": { \"type\": \"string\", \"description\": \"Название компании\" }, \"industry\": { \"type\": \"string\", \"description\": \"Отрасль/сфера деятельности\" }, \"mainProcurementItems\": { \"type\": \"array\", \"description\": \"Список основных товаров или услуг, которые закупает компания\", \"items\": { \"type\": \"string\" } } }, \"required\": [ \"companyName\", \"industry\", \"mainProcurementItems\" ], \"additionalProperties\": false }, \"painPoints\": { \"type\": \"array\", \"description\": \"Список основных болей в закупках\", \"items\": { \"type\": \"object\", \"properties\": { \"category\": { \"type\": \"string\", \"description\": \"Категория или тип боли\" }, \"description\": { \"type\": \"string\", \"description\": \"Подробное описание проблемы\" } }, \"required\": [ \"category\", \"description\" ], \"additionalProperties\": false } }, \"currentChallenges\": { \"type\": \"array\", \"description\": \"Актуальные вызовы/проблемы на данный момент\", \"items\": { \"type\": \"object\", \"properties\": { \"challenge\": { \"type\": \"string\", \"description\": \"Проблема, с которой столкнулась компания/респондент\" }, \"impact\": { \"type\": \"string\", \"description\": \"Как эта проблема влияет на процессы\" }, \"resolutionActions\": { \"type\": \"string\", \"description\": \"Какие меры предпринимаются для решения\" } }, \"required\": [ \"challenge\", \"impact\", \"resolutionActions\" ], \"additionalProperties\": false } }, \"needsAndWishes\": { \"type\": \"array\", \"description\": \"Потребности и пожелания респондента по улучшению процесса закупок\", \"items\": { \"type\": \"object\", \"properties\": { \"need\": { \"type\": \"string\", \"description\": \"Что респондент хочет улучшить\" }, \"priority\": { \"type\": \"string\", \"description\": \"Приоритет (высокий/средний/низкий)\" }, \"comment\": { \"type\": \"string\", \"description\": \"Дополнительный комментарий\" } }, \"required\": [ \"need\", \"priority\", \"comment\" ], \"additionalProperties\": false } }, \"supplierCriteria\": { \"type\": \"object\", \"description\": \"Критерии, по которым респондент выбирает поставщиков\", \"properties\": { \"priceImportance\": { \"type\": \"string\", \"description\": \"Важность цены\" }, \"qualityImportance\": { \"type\": \"string\", \"description\": \"Важность качества\" }, \"ecologyImportance\": { \"type\": \"string\", \"description\": \"Важность экологических аспектов\" }, \"innovationImportance\": { \"type\": \"string\", \"description\": \"Важность инноваций\" }, \"otherCriteria\": { \"type\": \"string\", \"description\": \"Другие критерии (если есть)\" } }, \"required\": [ \"priceImportance\", \"qualityImportance\", \"ecologyImportance\", \"innovationImportance\", \"otherCriteria\" ], \"additionalProperties\": false }, \"marketOpportunities\": { \"type\": \"array\", \"description\": \"Идеи респондента о том, чего не хватает на рынке\", \"items\": { \"type\": \"object\", \"properties\": { \"opportunity\": { \"type\": \"string\", \"description\": \"Описание того, чего не хватает\" }, \"comment\": { \"type\": \"string\", \"description\": \"Почему это важно\" } }, \"required\": [ \"opportunity\", \"comment\" ], \"additionalProperties\": false } }, \"conclusions\": { \"type\": \"object\", \"description\": \"Итоговые выводы и рекомендации\", \"properties\": { \"keyInsights\": { \"type\": \"array\", \"description\": \"Основные инсайты\", \"items\": { \"type\": \"string\" } }, \"recommendations\": { \"type\": \"array\", \"description\": \"Рекомендации\", \"items\": { \"type\": \"string\" } }, \"closingRemarks\": { \"type\": \"string\", \"description\": \"Заключительное слово/благодарность\" } }, \"required\": [ \"keyInsights\", \"recommendations\", \"closingRemarks\" ], \"additionalProperties\": false } }, \"required\": [ \"companyInfo\", \"painPoints\", \"currentChallenges\", \"needsAndWishes\", \"supplierCriteria\", \"marketOpportunities\", \"conclusions\" ], \"additionalProperties\": false } }";
        }
    }
}
