package com.greedytown.domain.social.service;

import com.greedytown.domain.social.dto.MessageDto;
import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.social.dto.MyMessageDto;
import com.greedytown.domain.social.dto.RankingDto;
import com.greedytown.domain.user.model.User;

import java.util.List;

public interface SocialService {

    List<RankingDto> getUserRanking();

    Void insertFriend(User user, Long friendSeq);

    Boolean isFriend(User user,Long friendSeq);

    List<MyFriendDto> getMyFriendList(User user);

    List<MyFriendDto> deleteMyFriend(User user,Long friendSeq);

    Void sendMessage(User user, MessageDto messageDto);

    List<MyMessageDto> getMyMessage(User user);

    Void deleteMessage(Long messageSeq);

    Void deleteAllMessage(User user);

    Long getMyNewMessage(User user);

    List<MyFriendDto> getMyFriendAlarmList(User user);

    Void deleteMyFriendAlarmList(User user, Long fromFriend);
}
