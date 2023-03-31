package com.greedytown.domain.item.controller;

import com.greedytown.domain.item.dto.*;
import com.greedytown.domain.item.model.Achievements;
import com.greedytown.domain.item.service.ItemService;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import com.greedytown.domain.user.service.UserService;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiParam;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;
import java.util.List;

@RestController
@RequestMapping("/item")
@RequiredArgsConstructor
public class ItemController {

    @Autowired
    private ItemService itemService;

    @Autowired
    public ItemController(ItemService itemService) {this.itemService = itemService;}


    @Transactional
    @ApiOperation(value = "상점에 있는 아이템 전부를 조회한다.", notes = "아이템을 조회해 보자")
    @GetMapping("/market")
    public ResponseEntity<?> getStoreItems() throws Exception {
        return new ResponseEntity<List<ItemDto>>(itemService.getStoreItems(), HttpStatus.OK);
    }


    @Transactional
    @ApiOperation(value = "아이템을 구입한다.", notes = "아이템을 구입해보자.")
    @PostMapping("/market")
    public ResponseEntity<BuyItemReturnDto> buyStoreItem(@RequestBody @ApiParam(value = "아이템 정보.", required = true) BuyItemDto buyItemDto, HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<BuyItemReturnDto>(itemService.buyStoreItem(buyItemDto, user), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "내 아이템을 조회한다.", notes = "아이템을 조회해보자.")
    @GetMapping("/character-custom")
    public ResponseEntity<BuyItemReturnDto> getMyItemList(HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<BuyItemReturnDto>(itemService.getMyItemList(user), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "내 업적을 조회한다.", notes = "업적을 조회해보자.")
    @GetMapping("/achievement")
    public ResponseEntity<?> getMyAchievements(HttpServletRequest request) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<List<AchievementsDto>>(itemService.getMyAchievements(user), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "업적을 등록한다.", notes = "업적을 등록해보자.")
    @PostMapping("/achievement")
    public ResponseEntity<?> insertMyAchievements(HttpServletRequest request,Long AchievementsIndex) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<List<AchievementsDto>>(itemService.insertMyAchievements(user,AchievementsIndex), HttpStatus.OK);
    }

    @Transactional
    @ApiOperation(value = "캐릭터 커스터마이징 한다.", notes = "캐릭터를 꾸며보자.")
    @PostMapping("/character-custom")
    public ResponseEntity<?> changeMyDress(HttpServletRequest request, @RequestBody @ApiParam(value = "아이템 정보.", required = true)List<WearingDto> wearingDto) throws Exception {
        User user = (User) request.getAttribute("USER");
        return new ResponseEntity<BuyItemReturnDto>(itemService.changeMyDress(user,wearingDto), HttpStatus.OK);
    }




}
