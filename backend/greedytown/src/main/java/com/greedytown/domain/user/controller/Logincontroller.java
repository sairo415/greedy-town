package com.greedytown.domain.user.controller;

import com.greedytown.domain.user.dto.LoginRequestDto;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.service.UserService;
import io.swagger.annotations.ApiOperation;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.Map;

@RestController
@RequiredArgsConstructor
public class Logincontroller {

    private final UserService userService;

    @GetMapping
    public ResponseEntity<?> aliveCheck() {
        return new ResponseEntity<>("Alive", HttpStatus.OK);
    }

    // 이메일 중복 확인
    @PostMapping("/check-email")
    @ApiOperation(value = "이메일 중복 확인", notes = "사용 중인 이메일인지 확인한다.")
    public ResponseEntity<?> duplicatedEmail(@RequestBody String userEmail) {
        Map<String, String> response = new HashMap<>();
        if(!userService.duplicatedEmail(userEmail)) {
            response.put("message", "사용 가능한 이메일");
        } else {
            response.put("message", "사용 중인 이메일");
        }
        return new ResponseEntity<>(response, HttpStatus.OK);
    }

    // 닉네임 중복 확인
    @PostMapping("/check-nickname")
    @ApiOperation(value = "닉네임 중복 확인", notes = "사용 중인 닉네임인지 확인한다.")
    public ResponseEntity<?> duplicatedNickname(@RequestBody String userNickname) {
        Map<String, String> response = new HashMap<>();
        if(!userService.duplicatedNickname(userNickname)) {
            response.put("message", "사용 가능한 닉네임");
        } else {
            response.put("message", "사용 중인 닉네임");
        }
        return new ResponseEntity<>(response, HttpStatus.OK);
    }

    @PostMapping("/regist")
    @ApiOperation(value = "회원 가입", notes = "회원가입을 한다.")
    public ResponseEntity<?> regist(@RequestBody UserDto userDto) {
        Map<String, String> response = new HashMap<>();
        boolean success = userService.insertUser(userDto);
        if (success) {
            response.put("message", "success");
        } else {
            response.put("message", "fail");
        }
        return new ResponseEntity<>(response, HttpStatus.OK);
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequestDto loginRequestDto) {
        return new ResponseEntity<>(HttpStatus.OK);
    }
}
