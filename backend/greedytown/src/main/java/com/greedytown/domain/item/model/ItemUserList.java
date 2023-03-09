package com.greedytown.domain.item.model;

import com.greedytown.domain.user.model.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
@IdClass(ItemUserListPK.class)
@AllArgsConstructor
@NoArgsConstructor
public class ItemUserList {

    @Id
    @ManyToOne
    @JoinColumn(name="user_index")
    private User userIndex;

    @Id
    @ManyToOne
    @JoinColumn(name="item_index")
    private Item itemIndex;



}
