package com.greedytown.domain.user.controller;

import com.greedytown.domain.social.dto.FriendUserListDto;
import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.user.dto.StatDto;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.service.UserService;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiParam;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;

@RestController("/user")
@RequiredArgsConstructor
public class UserController {

    private final UserService userService;


    @Transactional
    @ApiOperation(value = "스탯 업데이트", notes = "스탯 업데이트를 해보자.")
    @PatchMapping("/stat")
    public ResponseEntity<?> updateStat(HttpServletRequest request, @RequestBody @ApiParam(value = "아이템 정보.", required = true) StatDto statDto) {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<StatDto>(userService.updateStat(user,statDto), HttpStatus.OK);
    }


}
