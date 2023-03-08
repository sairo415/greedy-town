package com.greedytown.global.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import springfox.documentation.builders.ApiInfoBuilder;
import springfox.documentation.builders.PathSelectors;
import springfox.documentation.builders.RequestHandlerSelectors;
import springfox.documentation.service.ApiInfo;
import springfox.documentation.spi.DocumentationType;
import springfox.documentation.spring.web.plugins.Docket;

import java.util.function.Predicate;

@Configuration
public class SwaggerConfig {


    private ApiInfo apiInfo() {
        return new ApiInfoBuilder()
                .title("Greedy Town Spring Boot REST API")
                .version("1.0.0")
                .description("Welcome to Greedy Town")
                .build();
    }

    @Bean
    public Docket api() {
        return new Docket(DocumentationType.OAS_30)
                .select()
                .apis(RequestHandlerSelectors.basePackage("com.greedytown.domain"))
                .paths(PathSelectors.any())
                .build()
                ;
    }



}
