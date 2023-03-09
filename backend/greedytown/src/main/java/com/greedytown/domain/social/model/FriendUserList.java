package com.greedytown.domain.social.model;

import com.greedytown.domain.item.model.ItemUserListPK;
import com.greedytown.domain.user.model.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
@IdClass(FriendUserListPK.class)
@AllArgsConstructor
@NoArgsConstructor
public class FriendUserList {

    @Id
    @ManyToOne
    @JoinColumn(name="user_index_a")
    private User userIndexA;

    @Id
    @ManyToOne
    @JoinColumn(name="user_index_b")
    private User userIndexB;




}
