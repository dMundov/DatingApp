namespace API.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    
    using API.Data.Entities;
    using API.DTos;
    using API.Extensions;
    using API.Helpers;
    using API.Interfaces;

    public class MessagesController : BaseApiController
    {
        
        private readonly IMapper _mapper;
        public IUnitOfWork _uow;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
           
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if(username == createMessageDto.RecipientUserName.ToLower())
                return BadRequest("You cannot send messages to yourself!");

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUserName);

            if(recipient == null) return NotFound();

            var message = new Message
            {
                Sender=sender,
                Recipient=recipient,
                SenderUserName = sender.UserName,
                RecipientUserName= recipient.UserName,
                Content= createMessageDto.Content
            };


            _uow.MessageRepository.AddMessage(message);

            if(await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Fail to send Message!");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize,messages.TotalCount,messages.TotalPages);

            return messages;
        }

        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await _uow.MessageRepository.GetMessage(id);

            if(message.SenderUserName != username && message.RecipientUserName != username)            
                return Unauthorized();

            if(message.SenderUserName == username) message.SenderDeleted = true;
            if(message.RecipientUserName == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if(await _uow.Complete()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }

}