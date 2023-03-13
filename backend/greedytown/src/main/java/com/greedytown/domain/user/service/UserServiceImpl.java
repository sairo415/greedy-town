package com.greedytown.domain.user.service;

import com.auth0.jwt.JWT;
import com.auth0.jwt.algorithms.Algorithm;
import com.auth0.jwt.interfaces.DecodedJWT;
import com.greedytown.domain.item.model.Wearing;
import com.greedytown.domain.item.repository.WearingRepository;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import com.greedytown.global.config.jwt.JwtProperties;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.Date;
import java.util.HashMap;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final BCryptPasswordEncoder bCryptPasswordEncoder;

    private final WearingRepository wearingRepository;

    @Override
    public String insertUser(UserDto userDto) {
        User wearingUser = null;
        String message = "";
        userDto.setUserPassword(bCryptPasswordEncoder.encode(userDto.getUserPassword()));
        User user = User.builder()
                .userNickname(userDto.getUserNickname())
                .userEmail(userDto.getUserEmail())
                .userPassword(userDto.getUserPassword())
                .build();
        try {
            wearingUser = userRepository.save(user);
        } catch (Exception e) {
            message = "회원가입 실패";
            return message;
        }
        try {
            Wearing wearing = new Wearing();
            wearing.setUserSeq(wearingUser);
            wearingRepository.save(wearing);

        } catch (Exception e){
            message = "옷입기 실패";
            return message;
        }
        message = "다 성공";

        return message;
    }



    @Override
    public boolean duplicatedEmail(String userEmail) {
        User user = userRepository.findByUserEmail(userEmail);
        if(user == null) return false;
        return true;
    }

    @Override
    public boolean duplicatedNickname(String userNickname) {
        User user = userRepository.findByUserNickname(userNickname);
        if(user == null) return false;
        return true;
    }

    @Override
    public Map<String, String> reissue(String refreshToken) {
        Map<String, String> response = new HashMap<>();
        DecodedJWT refreshJwt = JWT.require(Algorithm.HMAC512(JwtProperties.SECRET)).build().verify(refreshToken);
        Date expiration = refreshJwt.getExpiresAt();
        long now = new Date().getTime();
        if(expiration.getTime() - now < 0) { // 만료되었으면
            response.put("message", "리프레시 토큰 만료");
        }
        // 레디스에서 리프레시 토큰 찾기

        // 로그아웃해서 레디스에 리프레시 토큰이 없으면
        // 레디스에 저장된 리프레시 토큰과 일치하지 않으면

        // access token 재발급
        return response;
    }
}
