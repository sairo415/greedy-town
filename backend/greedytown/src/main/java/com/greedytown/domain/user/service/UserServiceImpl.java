package com.greedytown.domain.user.service;

import com.auth0.jwt.JWT;
import com.auth0.jwt.algorithms.Algorithm;
import com.auth0.jwt.interfaces.DecodedJWT;
import com.greedytown.domain.item.model.Wearing;
import com.greedytown.domain.item.repository.WearingRepository;
import com.greedytown.domain.social.model.Stat;
import com.greedytown.domain.user.dto.TokenDto;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import com.greedytown.global.config.jwt.JwtProperties;
import lombok.RequiredArgsConstructor;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.util.ObjectUtils;

import javax.transaction.Transactional;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.TimeUnit;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final BCryptPasswordEncoder bCryptPasswordEncoder;

    private final WearingRepository wearingRepository;
    private final RedisTemplate redisTemplate;

    @Override
    public String insertUser(UserDto userDto) {
        User creatingUser = null;
        String message = "";
        userDto.setUserPassword(bCryptPasswordEncoder.encode(userDto.getUserPassword()));
        User user = User.builder()
                .userNickname(userDto.getUserNickname())
                .userEmail(userDto.getUserEmail())
                .userPassword(userDto.getUserPassword())
                .build();
        try {
            creatingUser = userRepository.save(user);
        } catch (Exception e) {
            message = "회원가입 실패";
            return message;
        }
        try {
            Stat stat = new Stat();
            stat.setUserSeq(creatingUser.getUserSeq());
            stat.setUserClearTime(null);

        } catch (Exception e){
            message = "스탯 실패";
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
    public Map<String, String> reissue(TokenDto tokenDto) {
        Map<String, String> response = new HashMap<>();
        DecodedJWT refreshJwt = JWT.require(Algorithm.HMAC512(JwtProperties.SECRET)).build().verify(tokenDto.getRefreshToken());
        Date expiration = refreshJwt.getExpiresAt();
        long now = new Date().getTime();
        if(expiration.getTime() - now < 0) { // 만료되었으면
            response.put("message", "리프레시 토큰 만료");
        }
        // 레디스에서 리프레시 토큰 찾기
        DecodedJWT accessJwt = JWT.require(Algorithm.HMAC512(JwtProperties.SECRET)).build().verify(tokenDto.getAccessToken());
        String userEmail1 = accessJwt.getClaim("username").toString();
        String userEmail2 = accessJwt.getClaim("username").asString();
        System.out.println(userEmail1);
        System.out.println(userEmail2);
        User user = userRepository.findByUserEmail(userEmail1);
        String refreshToken = (String)redisTemplate.opsForValue().get("RT:" + userEmail1);

        // 로그아웃해서 레디스에 리프레시 토큰이 없으면
        if(ObjectUtils.isEmpty(refreshToken)) {
            response.put("message", "Refresh Token 정보가 유효하지 않습니다.");
            return response;
        }
        // 레디스에 저장된 리프레시 토큰과 일치하지 않으면
        if(!refreshToken.equals(tokenDto.getRefreshToken())) {
            response.put("message", "Refresh Token 정보가 일치하지 않습니다.");
            return response;
        }
        // access token 재발급
        String accessToken = JWT.create()
                .withSubject(userEmail1)
                .withExpiresAt(new Date(System.currentTimeMillis()+JwtProperties.ACCESS_EXPIRATION_TIME))
                .withClaim("id", user.getUserSeq())
                .withClaim("username", userEmail1)
                .sign(Algorithm.HMAC512(JwtProperties.SECRET));
        response.put("message", "success");
        response.put("accessToken", JwtProperties.TOKEN_PREFIX+accessToken);
        return response;
    }

    @Override
    @Transactional
    public Map<String, String> logout(User user, String accessToken) {
        Map<String, String> response = new HashMap<>();

        String userEmail = user.getUserEmail();
        accessToken = accessToken.replace(JwtProperties.TOKEN_PREFIX, "");

        try {
            // Redis에서 User email로 저장된 Refresh Token이 있는지 확인 후 있을면 삭제한다.
            if (null != redisTemplate.opsForValue().get("RT:"+userEmail)){
                // Refresh Token을 삭제
                redisTemplate.delete("RT:"+userEmail);
            }

            // 해당 Access Token 유효시간을 가지고 와서 BlackList에 저장하기
            DecodedJWT accessJwt = JWT.require(Algorithm.HMAC512(JwtProperties.SECRET)).build().verify(accessToken);
            long expirationAt = accessJwt.getExpiresAt().getTime();
            long now = new Date().getTime();

            long expiration = expirationAt - now;
            redisTemplate.opsForValue().set(accessToken,"logout", expiration, TimeUnit.MILLISECONDS);
            response.put("message", "success");
            return response;
        } catch (Exception e) {
            response.put("message", "fail");
            return response;
        }
    }
}
