package com.greedytown.domain.user.controller;

import com.greedytown.domain.user.dto.LoginRequestDto;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.web.bind.annotation.*;

@RestController
//@RequestMapping("")
@RequiredArgsConstructor
public class Logincontroller {

    private final UserRepository userRepository;
    private final BCryptPasswordEncoder bCryptPasswordEncoder;

    @GetMapping
    public ResponseEntity<?> aliveCheck() {
        return new ResponseEntity<>("Alive", HttpStatus.OK);
    }

    /**
        회원가입
     */
    @PostMapping("/regist")
    public ResponseEntity<?> regist(@RequestBody UserDto userDto) {
        userDto.setUserPassword(bCryptPasswordEncoder.encode(userDto.getUserPassword()));
//        boolean success = userService.registUser(memberInfo);
        User user = User.builder()
                .userNickname(userDto.getUserNickname())
                .userEmail(userDto.getUserEmail())
                .userPassword(userDto.getUserPassword())
                .build();
        userRepository.save(user);
        return new ResponseEntity<>(HttpStatus.CREATED);
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequestDto loginRequestDto) {
        return new ResponseEntity<>(HttpStatus.OK);
    }
}
