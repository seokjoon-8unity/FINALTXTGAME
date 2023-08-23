namespace SpartaDungeonPractice
{
    internal class Program
    {
        private static PlayerStat _player;
        private static ItemData _itemData;
        static List<ItemData> _itemsInData = new List<ItemData>();
        static List<ItemData> _playerEquippedItems = new List<ItemData>();

        static void Main(string[] args)
        {
            ItemData();
            PlayerData();
            MainGame();
        }
        static void PlayerData()
        {
            Console.Title = "닉네임 설정";
            Console.WriteLine("닉네임 입력");
            Console.Write(">>");

            string inputName = Console.ReadLine();

            if (!string.IsNullOrEmpty(inputName))
            {
                Console.Clear();
                _player = new PlayerStat($"{inputName}", "전사", 1, 10, 5, 100, 1500);
            }
            else
            {
                Console.WriteLine("닉네임을 입력해주세요");
            }
        }
        static void ItemData()
        {
            _itemsInData.Add(new ItemData(0, "검", 1, 0, "단단한 검.", 100, true));
            _itemsInData.Add(new ItemData(1, "갑옷", 0, 1, "단단한 갑옷.", 100, true));

        }
        static void MainGame()
        {
            Console.Title = "스파르타 던전";
            Color(ConsoleColor.Red);
            Console.WriteLine("Sparta Dungeon Game!");
            Console.ResetColor();
            Color(ConsoleColor.Blue);
            Console.Write($"{_player.Name} ");
            Console.ResetColor();
            Console.WriteLine("님, 스파르타 마을에 오신것을 환영합니다!");
            Console.WriteLine("이곳에서 던전으로 돌아가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("0. 게임 종료 ");
            int input = CheckValidAction(0, 2);

            switch (input)
            {
                case 1:
                    DisplayPlayerState();
                    break;
                case 2:
                    DisplayPlayerInventory();
                    break;
                case 0:
                    Environment.Exit(0);
                    break;
            }
        }
        static void DisplayPlayerInventory()
        {
            Console.Clear();
            Console.Title = "인벤토리";
            Console.WriteLine("[인벤토리]");
            Console.WriteLine("보유 중인 아이템을 장착,해제할 수 있습니다.");
            Console.WriteLine("[아이템 목록]");

            string _itemEquipped;
            for (int i = 0; i < _itemsInData.Count; i++)
            {
                ItemData _item = _itemsInData[i];

                if (!_item.IsPlayerOwned)
                {
                    continue; 
                }
                _itemEquipped = _item.IsItemEquipped ? "[E] " : "";

                string _itemName = FormatAndPad(_item.ItemName, 10);
                string _itemComm = FormatAndPad(_item.ItemComm, 10);

                Console.Write($"{_item.ItemId + 1} | ");
                if (_item.IsItemEquipped)
                {
                    Color(ConsoleColor.Red);
                }
                Console.Write($"{_itemEquipped}");
                Console.ResetColor();
                Console.Write($"{_itemName}");
                DisplayAtkorDef(_item);
                Console.WriteLine($" {_itemComm} ");
            }
            Console.WriteLine(" ");
            Console.WriteLine("1. 보유 아이템 장착하기");
            Console.WriteLine("0. 나가기");

            int _input = CheckValidAction(0, 1);

            switch (_input)
            {
                case 0:
                    Console.Clear();
                    MainGame();
                    break;
                case 1:
                    Console.Clear();
                    PlayerInventory();
                    break;
            }
        }
        static void PlayerInventory()
        {
            Console.Clear();
            Console.Title = "인벤토리 - 장착여부";
            Console.WriteLine("[인벤토리 - 장착여부]");
            Console.WriteLine("보유 중인 아이템을 장착,해제할 수 있습니다.");
            Console.WriteLine("[아이템 목록]");
            string _itemEquipped;
            for (int i = 0; i < _itemsInData.Count; i++)
            {
                ItemData _item = _itemsInData[i];

                if (!_item.IsPlayerOwned)
                {
                    continue; 
                }

                _itemEquipped = _item.IsItemEquipped ? "[E] " : "";
                string _itemName = FormatAndPad(_item.ItemName, 10);
                string _itemComm = FormatAndPad(_item.ItemComm, 10);
                Console.Write($"{_item.ItemId + 1} | ");
                if (_item.IsItemEquipped)
                {
                    Color(ConsoleColor.Red);
                }
                Console.Write($"{_itemEquipped}");
                Console.ResetColor();
                Console.Write($"{_itemName}");
                DisplayAtkorDef(_item);
                Console.WriteLine($" {_itemComm} ");
            }
            Console.WriteLine(" ");
            Console.WriteLine("0. 나가기");
            int _input = CheckValidAction(0, _itemsInData.Count);

            if (_input == 0)
            {
                Console.Clear();
                DisplayPlayerInventory();
            }
            else if (_input > 0 && _input <= _itemsInData.Count)
            {
                ItemData _selectedItem = _itemsInData[_input - 1];
                ToggleEquip(_selectedItem);
                PlayerInventory();
            }
        }
        static string FormatAndPad(string _text, int _width)
        {
            int _remainingSpace = _width - _text.Length;
            if (_remainingSpace <= 0)
            {
                return _text;
            }
            else
            {
                int _leftPadding = _remainingSpace / 2;
                int _rightPadding = _remainingSpace - _leftPadding;
                string _formattedText = new string(' ', 2 * _leftPadding) + _text + new string(' ', 2 * _rightPadding);
                return _formattedText;
            }
        }
        static void ToggleEquip(ItemData _item)
        {
            bool _isAtkItemEquipped = IsAtkItemEquipped();
            bool _isDefItemEquipped = IsDefItemEquipped();

            if (_item.ItemAtk > 0 && _isAtkItemEquipped && !_item.IsItemEquipped)
            {
                for (int i = _playerEquippedItems.Count - 1; i >= 0; i--)
                {
                    ItemData item = _playerEquippedItems[i];
                    if (item.IsItemEquipped && item.ItemAtk > 0)
                    {
                        item.IsItemEquipped = false;
                        _playerEquippedItems.RemoveAt(i);
                        break;
                    }
                }
            }

            _item.IsItemEquipped = !_item.IsItemEquipped;

            if (_item.IsItemEquipped)
            {
                _playerEquippedItems.Add(_item);
            }
            else
            {
                _playerEquippedItems.Remove(_item);
            }
            UpdatePlayerStats();
        }
        static void UpdatePlayerStats()
        {
            int _totalAtk = 0;
            int _totalDef = 0;

            foreach (ItemData _item in _playerEquippedItems)
            {
                _totalAtk += _item.ItemAtk;
                _totalDef += _item.ItemDef;
            }

            _player.Atk = _player.Atk + _totalAtk;
            _player.Def = _player.Def + _totalDef;
        }
        static void DisplayAtkorDef(ItemData _item)
        {
            if (_item.ItemAtk > 0 && _item.ItemDef == 0)
            {
                string _itemAtk = FormatAndPad(_item.ItemAtk.ToString(), 1);
                Console.Write($"| 공격력 + {_itemAtk} |");
            }
            else if (_item.ItemAtk == 0 && _item.ItemDef > 0)
            {
                string _itemDef = FormatAndPad(_item.ItemDef.ToString(), 1);
                Console.Write($"| 방어력 + {_itemDef} |");
            }
        }
        static bool IsAtkItemEquipped()
        {
            foreach (ItemData _tem in _playerEquippedItems)
            {
                if (_tem.ItemAtk > 0)
                {
                    return true;
                }
            }
            return false;
        }
        static bool IsDefItemEquipped()
        {
            foreach (ItemData _item in _playerEquippedItems)
            {
                if (_item.ItemDef > 0)
                {
                    return true;
                }
            }
            return false;
        }
        static void DisplayPlayerState()
        {
            Console.Clear();
            Console.WriteLine($"상태보기");
            Console.WriteLine($"캐릭터의 정보가 표시됩니다.");
            Console.WriteLine($"Lv. {_player.Level}");
            Console.WriteLine($"{_player.Name} ( {_player.PlayerClass} )");
            Console.WriteLine($"공격력 : {_player.Atk}");
            Console.WriteLine($"방어력 : {_player.Def}");
            Console.WriteLine($"체 력 : {_player.Hp}"); ;
            Console.WriteLine($"Gold : {_player.Gold} G");
            Console.WriteLine(" ");
            Console.WriteLine("0. 나가기");
            int _input = CheckValidAction(0, 0);

            switch (_input)
            {
                case 0:
                    Console.Clear();
                    MainGame();
                    break;
            }
        }
        static int CheckValidAction(int _min, int _max)
        {
            while (true)
            {
                Console.WriteLine(" ");
                Console.WriteLine(" 원하시는 행동을 입력해주세요.");
                Console.Write(">>");
                string _input = Console.ReadLine();

                bool _parseSuccess = int.TryParse(_input, out var _ret);
                if (_parseSuccess)
                {
                    if (_ret >= _min && _ret <= _max)
                        return _ret;
                }
                Console.WriteLine("잘못된 입력입니다.");
            }
        }
        static void Color(ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
        }
    }
    public class PlayerStat
    {
        public string Name;
        public string PlayerClass;
        public int Level;
        public int Atk;
        public int Def;
        public int Hp;
        public int Gold;
        public int BaseAtk;
        public int BaseDef;
        public PlayerStat(string _name, string _playerClass, int _level, int _atk, int _def, int _hp, int _gold)
        {
            Name = _name;
            PlayerClass = _playerClass;
            Level = _level;
            Atk = _atk;
            Def = _def;
            Hp = _hp;
            Gold = _gold;
            BaseAtk = _atk;
            BaseDef = _def;
        }
    }
    public class ItemData
    {
        public int ItemId;
        public bool IsItemEquipped;
        public bool IsPlayerOwned;
        public string ItemName;
        public int ItemAtk;
        public int ItemDef;
        public string ItemComm;
        public int ItemPrice { get; set; }
        public ItemData(int _itemId, string _itemName, int _itemAtk, int _itemDef, string _itemComm, int _itemPrice, bool _isPlayerOwned)
        {
            ItemId = _itemId;
            ItemName = _itemName;
            ItemAtk = _itemAtk;
            ItemDef = _itemDef;
            ItemComm = _itemComm;
            ItemPrice = _itemPrice;
            IsItemEquipped = false;
            IsPlayerOwned = _isPlayerOwned;
        }
    }
}
