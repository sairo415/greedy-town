package com.greedytown.global.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.builders.WebSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter;
import org.springframework.security.config.http.SessionCreationPolicy;

@Configuration
public class SecurityConfig extends WebSecurityConfigurerAdapter {
    @Override
    protected void configure(HttpSecurity http) throws Exception {
        http
                .authorizeRequests() // HttpServletRequest를 사용하는 요청들에 대해 접근제한 설정
                .antMatchers("/").permitAll() // ""에 대한 요청은 인증 없이 접근 허용
                .anyRequest().authenticated(); // 이외의 요청은 모두 인증 필요
    }

    //    @Override
//    protected void configure(HttpSecurity httpSecurity) throws Exception {
//        httpSecurity
//                .httpBasic().disable()
//                .cors().disable()
//                .csrf().disable()
//                .sessionManagement().sessionCreationPolicy(SessionCreationPolicy.STATELESS)
//                .and()
//                .authorizeRequests()
//                .anyRequest().permitAll();
//    }
//
//    @Override
//    public void configure(WebSecurity webSecurity) {
//        webSecurity.ignoring().antMatchers("/v2/api-docs", "/swagger-resources/**",
//                "/swagger-ui/**", "/webjars/**", "/swagger/**", "/sign-api/exception");
//    }
}
