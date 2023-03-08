package com.greedytown.domain.item.controller;

import com.greedytown.domain.item.dto.BuyItemDto;
import com.greedytown.domain.item.dto.BuyItemReturnDto;
import com.greedytown.domain.item.dto.ItemDto;
import com.greedytown.domain.item.service.ItemService;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiParam;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.*;

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
    public void buyStoreItem(@RequestBody @ApiParam(value = "아이템 정보.", required = true) BuyItemDto buyItemDto) throws Exception {
        return new ResponseEntity<BuyItemReturnDto>(itemService.buyStoreItem(buyItemDto), HttpStatus.OK);
    }

}
