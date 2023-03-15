use greedytown;

show tables;


select * from stat;
select * from wearing;
select * from user;
select * from item;	
select * from item_user_list; 
select * from achievements;
select * from success_user_achievements;
select * from friend_user_list;


update user set user_money = 50000 where user_seq=1;

insert into user (user_email, user_password, user_nickname, user_money, user_clear_time) values("z@z.com","111","바간삼",400000,0);
insert into user (user_email, user_password, user_nickname, user_money, user_clear_time) values("a@a.com","222","체르니",300000,0);

insert into item (item_name,item_price,achievements_seq,item_color_seq,item_type_seq)values("powerSuite",6000,null,1,1);
insert into item (item_name,item_price,achievements_seq,item_color_seq,item_type_seq)values("dragonArms",8000,null,1,1);
insert into item (item_name,item_price,achievements_seq,item_color_seq,item_type_seq)values("pinkpink",8000,null,null,null);

select * from achievements;

insert into item_type values(1,"arms");
insert into item_color values(1,"red");

select * from item_type;
select * from item;

insert into achievements values(1,"디토의 모자");

select * from item_user_list;

select * from message;

select * from wearing;

insert into friend_user_list values(2,1);

insert into item_user_list values(2,1);