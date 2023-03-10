package com.greedytown.domain.social.controller;

import com.greedytown.domain.item.dto.BuyItemDto;
import com.greedytown.domain.item.dto.BuyItemReturnDto;
import com.greedytown.domain.item.service.ItemService;
import com.greedytown.domain.social.dto.MessageDto;
import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.social.dto.MyMessageDto;
import com.greedytown.domain.social.dto.RankingDto;
import com.greedytown.domain.social.service.SocialService;
import com.greedytown.domain.user.model.User;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiParam;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.*;

import javax.print.attribute.standard.MediaSize;
import javax.servlet.http.HttpServletRequest;
import java.util.List;

@RestController
@RequestMapping("/social")
@RequiredArgsConstructor
public class SocialController {


    @Autowired
    private SocialService socialService;

    @Autowired
    public SocialController(SocialService socialService) {this.socialService = socialService;}


    @Transactional
    @ApiOperation(value = "랭킹을 본다.", notes = "전체 랭킹을 본다.")
    @GetMapping("/ranking")
    public ResponseEntity<List<RankingDto>> getUserRanking() throws Exception {
        return new ResponseEntity<List<RankingDto>>(socialService.getUserRanking(), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "친구를 추가한다.", notes = "친구를 추가해보자.")
    @PostMapping("/friend")
    public ResponseEntity<?> insertFriend(HttpServletRequest request, Long friendIndex) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<Void>(socialService.insertFriend(user,friendIndex), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "친구인지 확인한다.", notes = "전체 랭킹을 본다.")
    @GetMapping("/is-friend")
    public ResponseEntity<?> isFriend(HttpServletRequest request, Long friendIndex) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<Boolean>(socialService.isFriend(user,friendIndex), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "친구 목록을 본다.", notes = "친구 목록을 본다.")
    @GetMapping("/friend")
    public ResponseEntity<?> getMyFriendList(HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        System.out.println(user.getUserNickname()+" ???");
        return new ResponseEntity<List<MyFriendDto>>(socialService.getMyFriendList(user), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "친구를 삭제한다.", notes = "친구를 삭제해보자.")
    @DeleteMapping("/friend")
    public ResponseEntity<?> deleteMyFriend(HttpServletRequest request,Long friendIndex) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<List<MyFriendDto>>(socialService.deleteMyFriend(user,friendIndex), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "메세지를 보낸다.", notes = "친구한테 메세지를 보내보자.")
    @PostMapping("/message")
    public ResponseEntity<?> sendMessage(HttpServletRequest request, MessageDto messageDto) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<Void>(socialService.sendMessage(user,messageDto), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "내 메세지를 조회한다.", notes = "내 메세지를 보자.")
    @GetMapping("/message")
    public ResponseEntity<?> getMyMessage(HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<List<MyMessageDto>>(socialService.getMyMessage(user), HttpStatus.OK);
    }


    @Transactional
    @ApiOperation(value = "메세지를 삭제한다.", notes = "내 메세지를 삭제하자.")
    @DeleteMapping("/message")
    public ResponseEntity<?> deleteMessage(Long messageIndex) throws Exception {
        return new ResponseEntity<Void>(socialService.deleteMessage(messageIndex), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "내 메세지를 전부 삭제한다.", notes = "내 메세지를 전부 삭제하자.")
    @DeleteMapping("/delete-all-message")
    public ResponseEntity<?> deleteAllMessage(HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<Void>(socialService.deleteAllMessage(user), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "메세지 수를 본다.", notes = "읽지 않은 메세지 수가 몇개인지 한번 봐보자.")
    @GetMapping("/alarm")
    public ResponseEntity<?> getMyNewMessage(HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<Long>(socialService.getMyNewMessage(user), HttpStatus.OK);
    }



}
