package com.greedytown.global.config.jwt;

public interface JwtProperties {
    String SECRET = "greedy_town"; // 서버만 알고 있는 개인키
    int EXPIRATION_TIME = 1000 * 60; // 1분
    String TOKEN_PREFIX = "Bearer ";
    String HEADER_STRING = "Authorization";
}
