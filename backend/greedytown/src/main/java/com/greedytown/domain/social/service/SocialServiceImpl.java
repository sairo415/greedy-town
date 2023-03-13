package com.greedytown.domain.social.service;

import com.greedytown.domain.social.dto.MessageDto;
import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.social.dto.MyMessageDto;
import com.greedytown.domain.social.dto.RankingDto;
import com.greedytown.domain.social.model.FriendUserList;
import com.greedytown.domain.social.model.Message;
import com.greedytown.domain.social.model.Stat;
import com.greedytown.domain.social.repository.FriendUserListRepository;
import com.greedytown.domain.social.repository.MessageRepository;
import com.greedytown.domain.social.repository.StatRepository;
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

    private final StatRepository statRepository;

    //랭킹을 본다.
    @Override
    public List<RankingDto> getUserRanking() {
        List<RankingDto> list = new ArrayList<>();
        for(Stat stat : statRepository.findAllByOrderByUserClearTimeDesc()){
            User user = userRepository.findUserByUserSeq(stat.getUserSeq());
            RankingDto rankingDto = RankingDto.builder().
                                    userNickname(user.getUserNickname()).
                                    clearTime(stat.getUserClearTime().toString()).
                                    build();
            list.add(rankingDto);
        }
        return list;
    }

    @Override
    public Void insertFriend(User user, Long friendSeq) {
        FriendUserList friendUserList = new FriendUserList();
        User friend = userRepository.findUserByUserSeq(friendSeq);
        friendUserList.setFriendFrom(user);
        friendUserList.setFriendTo(friend);
        friendUserList.setFriendAccept(false);
        friendUserListRepository.save(friendUserList);
        return null;
    }

    @Override
    public Boolean isFriend(User user, Long friendSeq) {
        User friend = userRepository.findUserByUserSeq(friendSeq);
        Boolean check = friendUserListRepository.existsByFriendFrom_UserSeqAndFriendTo_UserSeqAndFriendAcceptIsTrue(user.getUserSeq(),friend.getUserSeq());
        if(check) return true;
//        user.getUserIndex();
        check = friendUserListRepository.existsByFriendTo_UserSeqAndFriendFrom_UserSeqAndFriendAcceptIsTrue(user.getUserSeq(),friend.getUserSeq());
        if(check) return true;
        return false;
    }

    @Override
    public List<MyFriendDto> getMyFriendList(User user) {

        return getMyFriends(user);

    }

    @Override
    public List<MyFriendDto> deleteMyFriend(User user, Long friendSeq) {

        friendUserListRepository.deleteByFriendFrom_userSeqAndFriendTo_userSeq(user.getUserSeq(),friendSeq);
        friendUserListRepository.deleteByFriendTo_userSeqAndFriendFrom_userSeq(user.getUserSeq(),friendSeq);

        return getMyFriends(user);
    }

    @Override
    public Void sendMessage(User user, MessageDto messageDto) {
        Message message = new Message();
        message.setMessageFrom(user);
        User friend = userRepository.findUserByUserSeq(messageDto.getMessageTo());
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

        for(Message message : messageRepository.findAllByMessageTo_UserSeq(user.getUserSeq())){
            User fromUser = userRepository.findUserByUserSeq(message.getMessageFrom().getUserSeq());
            MyMessageDto messageDto = MyMessageDto.builder().
                                      messageSeq(message.getMessageSeq()).
                                      messageContent(message.getMessageContent()).
                                      messageFrom(fromUser.getUserSeq()).
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
        messageRepository.deleteAllByMessageTo_UserSeq(user.getUserSeq());
        return null;
    }

    @Override
    public Long getMyNewMessage(User user) {

        return messageRepository.countAllByMessageTo_UserSeqAndMessageCheckFalse(user.getUserSeq());
    }


    @Override
    public List<MyFriendDto> getMyFriendAlarmList(User user) {
        List<MyFriendDto> myFriendDtos = new ArrayList<>();
        for(FriendUserList fu : friendUserListRepository.findAllByFriendTo_UserSeqAndFriendAcceptIsFalse(user.getUserSeq())){
            User fromUser = userRepository.findUserByUserSeq(fu.getFriendSeq());
            MyFriendDto myFriendDto = MyFriendDto.builder().
                                      userNickname(fromUser.getUserNickname()).
                                      userSeq(fromUser.getUserSeq()).
                                      build();
            myFriendDtos.add(myFriendDto);
        }
        return myFriendDtos;
    }

    @Override
    public Void deleteMyFriendAlarmList(User user, Long fromFriend) {
        friendUserListRepository.deleteByFriendFrom_userSeqAndFriendTo_userSeq(fromFriend , user.getUserSeq());
        return null;
    }


    //재사용을 위한 친구 보기

    private List<MyFriendDto> getMyFriends(User user){

        List<MyFriendDto> myFriendDtos = new ArrayList<>();

        for(FriendUserList friendUserList : friendUserListRepository.findAllByFriendFrom_UserSeqAndFriendAcceptIsTrue(user.getUserSeq())){
            User fromFriend = userRepository.findUserByUserSeq(friendUserList.getFriendTo().getUserSeq());
            MyFriendDto myFriendDto = MyFriendDto.builder().
                    userSeq(fromFriend.getUserSeq()).
                    userNickname(fromFriend.getUserNickname()).
                    build();
            myFriendDtos.add(myFriendDto);
        }
        for(FriendUserList friendUserList : friendUserListRepository.findAllByFriendTo_UserSeqAndFriendAcceptIsTrue(user.getUserSeq())){
            User toFriend = userRepository.findUserByUserSeq(friendUserList.getFriendFrom().getUserSeq());
            MyFriendDto myFriendDto = MyFriendDto.builder().
                    userSeq(toFriend.getUserSeq()).
                    userNickname(toFriend.getUserNickname()).
                    build();
            myFriendDtos.add(myFriendDto);
        }
        return myFriendDtos;
    }
}
