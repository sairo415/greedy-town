package com.greedytown.global.config.jwt;

public interface JwtProperties {
    String SECRET = "greedy_town"; // 서버만 알고 있는 개인키
    int ACCESS_EXPIRATION_TIME = 1000 * 60 * 30 ; // 30분
    long REFRESH_EXPIRATION_TIME = 1000 * 60 * 60 * 24 * 7 * 2; // 2주
    String TOKEN_PREFIX = "Bearer ";
    String HEADER_STRING = "Authorization";
}
