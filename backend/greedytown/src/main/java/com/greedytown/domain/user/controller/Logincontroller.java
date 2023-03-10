package com.greedytown.domain.user.controller;

import com.greedytown.domain.user.dto.LoginRequestDto;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.service.UserService;
import com.greedytown.global.config.jwt.JwtProperties;
import io.swagger.annotations.ApiOperation;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;
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
        String message = userService.insertUser(userDto);
        return new ResponseEntity<>(message, HttpStatus.OK);
    }

    @PostMapping("/login")
    @ApiOperation(value = "로그인", notes = "로그인을 한다.")
    public ResponseEntity<?> login(@RequestBody LoginRequestDto loginRequestDto) {
        return new ResponseEntity<>(HttpStatus.OK);
    }

    @PostMapping("/reissue")
    @ApiOperation(value = "토큰 재발급", notes = "accessToken을 재발급한다.")
    public ResponseEntity<?> reissueToken(HttpServletRequest request) {
        String refreshToken = request.getHeader(JwtProperties.HEADER_STRING);
        Map<String, String> response = new HashMap<>();
        if(refreshToken == null || !refreshToken.startsWith(JwtProperties.TOKEN_PREFIX)) {
            response.put("message", "리프레시 토큰 없음");
        } else {
            refreshToken = refreshToken.replace(JwtProperties.TOKEN_PREFIX, "");
            response = userService.reissue(refreshToken);
        }
        return new ResponseEntity<>(response, HttpStatus.OK);
    }

}
