﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTO;
using BL.DTO.Common;
using BL.DTO.Filters;
using BL.Facades.Common;
using BL.Services.Characters;
using BL.Services.GroupPosts;
using BL.Services.Groups;
using Game.Infrastructure.UnitOfWork;

namespace BL.Facades
{
    public class GroupFacade : FacadeBase
    {
        private readonly IGroupService _groupService;
        private readonly IGroupPostService _groupPostService;
        private readonly ICharacterService _characterService;

        public GroupFacade(IUnitOfWorkProvider unitOfWorkProvider, IGroupService groupService, IGroupPostService groupPostService, ICharacterService characterService) : base(unitOfWorkProvider)
        {
            _groupService = groupService;
            _groupPostService = groupPostService;
            _characterService = characterService;
        }

        public async Task<Guid> CreateGroup(Guid groupFounder,string name, string description, string imagePath)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var founder = await _characterService.GetAsync(groupFounder);
                if (founder == null)
                {
                    return Guid.Empty;
                }
                if (founder.Group != null)
                {
                    return Guid.Empty;
                }
                var group = new GroupDto
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    Picture = imagePath,
                };
                founder.IsGroupAdmin = true;
                var groupId = _groupService.Create(group);
                founder.GroupId = groupId;
                await _characterService.Update(founder);
                await uow.Commit();

                founder = _characterService.GetAsync(groupFounder).Result;
                return groupId; 
            }
        }

        public async void RemoveGroup(Guid groupId)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var group =_groupService.GetAsync(groupId).Result;
                if (group == null)
                    return;
                foreach (var member in group.Members)
                {
                    member.Group = null;
                    await _characterService.Update(member);
                }
                _groupService.Delete(groupId);
                await uow.Commit();
            }
        }

        public async void EditDescription(Guid groupId, string newDescription)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var group =_groupService.GetAsync(groupId).Result;
                if (group == null)
                    return;
                group.Description = newDescription;
                await _groupService.Update(group);
                await uow.Commit();
            }
        }

        public async Task<QueryResultDto<GroupDto, GroupFilterDto>> GetAllGroupsAsync()
        {
            using (UnitOfWorkProvider.Create())
            {
                return await _groupService.ListAllAsync();
            }
        }

        public async Task<QueryResultDto<GroupDto, GroupFilterDto>> GetGroupsByFilterAsync(GroupFilterDto filter)
        {
            using (UnitOfWorkProvider.Create())
            {
                return await _groupService.ListGroupsAsync(filter);
            }
        }

        public async Task<int> AddToGroup(Guid characterId, Guid groupId)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var group =_groupService.GetAsync(groupId).Result;
                var character = _characterService.GetAsync(characterId).Result;
                if (group == null || character == null)
                    return -1;
                if (character.Group != null)
                    return -2;
                group.Members.Add(character);
                character.Group = group;
                await _characterService.Update(character);
                await _groupService.Update(group);
                await uow.Commit();
                return 1;
            }
        }

        public async Task<bool> RemoveFromGroup(Guid characterId, Guid groupId)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var group =_groupService.GetAsync(groupId).Result;
                var character = _characterService.GetAsync(characterId).Result;
                if (group == null || character == null)
                    return false;
                if (character.Group != null)
                    return false;
                if (!group.Members.Contains(character))
                {
                    return false;
                }
                group.Members.Remove(character);
                character.Group = null;
                await _characterService.Update(character);
                await _groupService.Update(group);
                await uow.Commit();
                return true;
            }
        }

        public bool CreatePost(GroupPostDto groupPost)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                var post = _groupPostService.Create(groupPost);
                if (post.Equals(Guid.Empty))
                {
                    return false;
                }
                uow.Commit();
                return true;
            }
        }

        public async Task<QueryResultDto<GroupPostDto, GroupPostFilterDto>> GetAllGroupPosts(GroupPostFilterDto filter)
        {
            using (UnitOfWorkProvider.Create())
            {
                return await _groupPostService.ListGroupPostsAsync(filter);
            }
        }

        public void EditPost(GroupPostDto groupPost)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                _groupPostService.Update(groupPost);
                uow.Commit();
            }
        }
        
        public void DeletePost(Guid groupPostId)
        {
            using (var uow = UnitOfWorkProvider.Create())
            {
                _groupPostService.Delete(groupPostId);
                uow.Commit();
            }
        }
    }
}
