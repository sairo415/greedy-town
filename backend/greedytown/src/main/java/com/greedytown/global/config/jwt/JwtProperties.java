package com.greedytown.global.config.jwt;

public interface JwtProperties {
    String SECRET = "greedy_town"; // 서버만 알고 있는 개인키
    int ACCESS_EXPIRATION_TIME = 1000 * 60 * 10 ; // 10분
    int REFRESH_EXPIRATION_TIME = 1000 * 60 * 60 ; // 1시간
    String TOKEN_PREFIX = "Bearer ";
    String HEADER_STRING = "Authorization";
}
