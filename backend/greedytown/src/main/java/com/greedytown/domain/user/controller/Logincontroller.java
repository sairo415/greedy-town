package com.greedytown.domain.user.controller;

import com.greedytown.domain.user.dto.LoginRequestDto;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequiredArgsConstructor
public class Logincontroller {

    private final UserService userService;

    @GetMapping
    public ResponseEntity<?> aliveCheck() {
        return new ResponseEntity<>("Alive", HttpStatus.OK);
    }

    /**
        회원가입
     */
    @PostMapping("/regist")
    public ResponseEntity<?> regist(@RequestBody UserDto userDto) {
        boolean success = userService.insertUser(userDto);
        if (success) {
            return new ResponseEntity<>(HttpStatus.CREATED);
        } else {
            return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequestDto loginRequestDto) {
        return new ResponseEntity<>(HttpStatus.OK);
    }
}
