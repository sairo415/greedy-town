package com.greedytown.domain.social.service;

import com.greedytown.domain.item.dto.BuyItemDto;
import com.greedytown.domain.item.dto.BuyItemReturnDto;
import com.greedytown.domain.item.dto.ItemDto;
import com.greedytown.domain.item.model.Item;
import com.greedytown.domain.item.model.ItemUserList;
import com.greedytown.domain.item.repository.ItemRepository;
import com.greedytown.domain.item.repository.ItemUserListRepository;
import com.greedytown.domain.item.service.ItemService;
import com.greedytown.domain.social.dto.MessageDto;
import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.social.dto.MyMessageDto;
import com.greedytown.domain.social.dto.RankingDto;
import com.greedytown.domain.social.model.FriendUserList;
import com.greedytown.domain.social.model.FriendUserListPK;
import com.greedytown.domain.social.model.Message;
import com.greedytown.domain.social.repository.FriendUserListRepository;
import com.greedytown.domain.social.repository.MessageRepository;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Service
@RequiredArgsConstructor
public class SocialServiceImpl implements SocialService {

    private final UserRepository userRepository;
    private final FriendUserListRepository friendUserListRepository;

    private final MessageRepository messageRepository;

    //랭킹을 본다.
    @Override
    public List<RankingDto> getUserRanking() {
        List<RankingDto> list = new ArrayList<>();
        for(User user : userRepository.findAllByOrderByUserClearTimeDesc()){
            RankingDto rankingDto = RankingDto.builder().
                                    userNickname(user.getUserNickname()).
                                    clearTime(user.getUserClearTime().toString()).
                                    build();
            list.add(rankingDto);
        }
        return list;
    }

    @Override
    public Void insertFriend(User user, Long friendIndex) {
        FriendUserList friendUserList = new FriendUserList();
        User friend = userRepository.findUserByUserIndex(friendIndex);
        friendUserList.setUserIndexA(user);
        friendUserList.setUserIndexB(friend);
        friendUserListRepository.save(friendUserList);
        return null;
    }

    @Override
    public Boolean isFriend(User user, Long friendIndex) {
        User friend = userRepository.findUserByUserIndex(friendIndex);
        Boolean check = friendUserListRepository.existsByUserIndexA_UserIndexAndUserIndexB_UserIndex(user.getUserIndex(),friend.getUserIndex());
        if(check) return true;
//        user.getUserIndex();
        check = friendUserListRepository.existsByUserIndexB_UserIndexAndUserIndexA_UserIndex(user.getUserIndex(),friend.getUserIndex());
        if(check) return true;
        return false;
    }

    @Override
    public List<MyFriendDto> getMyFriendList(User user) {

        return getMyFriends(user);

    }

    @Override
    public List<MyFriendDto> deleteMyFriend(User user, Long frinedIndex) {

        friendUserListRepository.deleteByUserIndexA_userIndexAndUserIndexB_userIndex(user.getUserIndex(),frinedIndex);
        friendUserListRepository.deleteByUserIndexB_userIndexAndUserIndexA_userIndex(user.getUserIndex(),frinedIndex);

        return getMyFriends(user);
    }

    @Override
    public Void sendMessage(User user, MessageDto messageDto) {
        Message message = new Message();
        message.setMessageFrom(user);
        User friend = userRepository.findUserByUserIndex(messageDto.getMessageTo());
        message.setMessageFrom(user);
        message.setMessageTo(friend);
        message.setMessageContent(messageDto.getMessageContent());
        message.setMessageWriteTime(LocalDateTime.now());
        message.setMessageCheck(false);
        messageRepository.save(message);
        return null;
    }

    @Override
    public List<MyMessageDto> getMyMessage(User user) {

        List<MyMessageDto> list = new ArrayList<>();

        for(Message message : messageRepository.findAllByMessageTo_UserIndex(user.getUserIndex())){
            User fromUser = userRepository.findUserByUserIndex(message.getMessageFrom().getUserIndex());
            MyMessageDto messageDto = MyMessageDto.builder().
                                      messageIndex(message.getMessageIndex()).
                                      messageContent(message.getMessageContent()).
                                      messageFrom(fromUser.getUserIndex()).
                                      messageFromNickname(fromUser.getUserNickname()).
                                      build();
            list.add(messageDto);
            //메세지 읽음 표시를 한다.
            message.setMessageCheck(Boolean.TRUE);
        }

        return list;
    }

    @Override
    public Void deleteMessage(Long messageIndex) {
        messageRepository.deleteById(messageIndex);
        return null;
    }

    @Override
    public Void deleteAllMessage(User user) {
        messageRepository.deleteAllByMessageTo_UserIndex(user.getUserIndex());
        return null;
    }

    @Override
    public Long getMyNewMessage(User user) {

        return messageRepository.countAllByMessageTo_UserIndexAndMessageCheckFalse(user.getUserIndex());
    }


    //재사용을 위한 친구 보기

    private List<MyFriendDto> getMyFriends(User user){

        List<MyFriendDto> myFriendDtos = new ArrayList<>();

        for(FriendUserList friendUserList : friendUserListRepository.findAllByUserIndexA_userIndex(user.getUserIndex())){
            User user1 = userRepository.findUserByUserIndex(friendUserList.getUserIndexB().getUserIndex());
            MyFriendDto myFriendDto = MyFriendDto.builder().
                    userIndex(user1.getUserIndex()).
                    userNickname(user1.getUserNickname()).
                    build();
            myFriendDtos.add(myFriendDto);
        }
        for(FriendUserList friendUserList : friendUserListRepository.findAllByUserIndexB_userIndex(user.getUserIndex())){
            User user1 = userRepository.findUserByUserIndex(friendUserList.getUserIndexA().getUserIndex());
            MyFriendDto myFriendDto = MyFriendDto.builder().
                    userIndex(user1.getUserIndex()).
                    userNickname(user1.getUserNickname()).
                    build();
            myFriendDtos.add(myFriendDto);
        }
        return myFriendDtos;
    }
}
