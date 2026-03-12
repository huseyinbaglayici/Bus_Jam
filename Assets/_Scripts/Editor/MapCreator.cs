using System.Collections.Generic;
using _Scripts.Editor.Utils;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Enums;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Editor
{
    public class MapCreator : EditorWindow
    {
        #region Constants

        private const string TitleColorHex = "#FF9F00";
        private const string GridBtnTextColorHex = "#FFFFFF";
        private const string GridBtnBackgroundColorHex = "#00C2FF";
        private const string SaveBtnBackgroundColorHex = "#39B33F";
        private const string DeleteBtnBackgroundColorHex = "#E55353";
        private const string SoDataPath = "Assets/Resources/Data/SO_Level_Data";
        private const string HeaderTitleText = "Bus Jam Level Creator";
        private const string HelpBoxText = "Enter parameters to initialize grid.";
        private const string GridBtnText = "Draw Grid";
        private const string BusLineBtnText = "Draw Bus Line";
        private const string PassengerLineBtnText = "Set Passenger Line";
        private const string GetLevelNumberText = "Existing Level Id";
        private const string GetLevelBtnText = "Get Existing Level";
        private const string DeleteSelectedLevelBtnText = "Delete Selected Level";
        private const int DefaultIntValue = 5;
        private const int DefaultTimerValue = 45;
        private const float DefaultMargin = 10f;

        private static readonly Dictionary<EntityColor, (Color bg, Color text)> ColorLookup = new()
        {
            { EntityColor.White, (Color.white, Color.black) },
            { EntityColor.DarkBlue, (new Color(0.1f, 0.23f, 0.42f), Color.white) },
            { EntityColor.LightBlue, (new Color(0.36f, 0.78f, 0.96f), Color.black) },
            { EntityColor.Yellow, (Color.yellow, Color.black) },
            { EntityColor.LightGreen, (new Color(0.48f, 0.78f, 0.49f), Color.black) },
            { EntityColor.DarkGreen, (new Color(0.18f, 0.42f, 0.31f), Color.white) },
            { EntityColor.Pink, (Color.magenta, Color.white) },
            { EntityColor.Purple, (new Color(0.5f, 0f, 0.5f), Color.white) },
            { EntityColor.Orange, (new Color(1f, 0.5f, 0f), Color.white) },
            { EntityColor.Red, (Color.red, Color.white) },
            { EntityColor.Default, (new Color(0.15f, 0.15f, 0.15f), Color.white) }
        };

        #endregion

        #region Fields

        private IntegerField _xField, _yField, _busLineField, _passengerLineField, _timerLineField, _existingLevelField;
        private CellSaveData[,] _cellDataMatrix;
        private List<BusLineSaveData> _busSequenceList = new();
        private OccupantType _brushOccupant = OccupantType.Empty;
        private EntityColor _brushColor = EntityColor.Default;
        private EntityColor _brushColorSelection = EntityColor.Default;
        private bool _isPainting;
        private bool _isEditMode;
        private int _currentLevelId = -1;
        private ScrollView _mainScrollView;
        private VisualElement _busLineContainer;
        private VisualElement _passengerLineContainer;
        private VisualElement _gridContainer;
        private Label _passengerCountLabel;

        #endregion

        [MenuItem("CustomTools/BusJam Level Creation Tool")]
        public static void ShowWindow() => GetWindow<MapCreator>();

        public void CreateGUI()
        {
            var root = rootVisualElement;
            SetupHeaderAndSettings(root);
            SetupDrawingArea(root);
            root.Add(CreateFooterSection());
            root.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (evt.button == 0) _isPainting = false;
            });
        }

        #region Setup

        private void SetupHeaderAndSettings(VisualElement root)
        {
            root.Add(CreateHeaderSection(HeaderTitleText));
            root.Add(CreatePathSection());
            root.Add(new HelpBox(HelpBoxText, HelpBoxMessageType.Info));
            root.Add(CreateEditExistingLevelSection());
            root.Add(CreatePaletteSection());
            root.Add(CreateLevelSettingsSection());
            root.Add(CreateActionButtonRow());
        }

        private void SetupDrawingArea(VisualElement root)
        {
            _mainScrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal)
            {
                style =
                {
                    marginTop = DefaultMargin, flexGrow = 1,
                    borderTopWidth = 1, borderTopColor = new Color(0.15f, 0.15f, 0.15f)
                }
            };

            _busLineContainer = CreateHorizontalContainer(10f, null, Justify.FlexStart);
            _passengerLineContainer = CreateHorizontalContainer(5f, new Color(0.1f, 0.1f, 0.1f, 0.3f));
            _passengerLineContainer.style.height = 25;
            _passengerLineContainer.style.display = DisplayStyle.None;

            _passengerCountLabel = new Label();
            _passengerLineContainer.Add(_passengerCountLabel);

            _gridContainer = new VisualElement { style = { marginTop = 15, alignItems = Align.Center } };

            _mainScrollView.Add(_busLineContainer);
            _mainScrollView.Add(_passengerLineContainer);
            _mainScrollView.Add(_gridContainer);
            root.Add(_mainScrollView);
        }

        #endregion

        #region Actions

        private void GetDesiredLevel(int levelNumber)
        {
            LevelDataSO loadedLevel = LevelSaveUtility.GetSelectedLevel(levelNumber, SoDataPath);
            if (!loadedLevel) return;
            _isEditMode = true;
            _currentLevelId = levelNumber;
            LoadLevelDataIntoUI(loadedLevel);
        }

        private void DeleteDesiredLevel(int levelNumber)
        {
            if (!LevelSaveUtility.DeleteSelectedLevel(levelNumber, SoDataPath)) return;
            if (!_isEditMode || _currentLevelId != levelNumber) return;

            _isEditMode = false;
            _gridContainer.Clear();
            _busLineContainer.Clear();
            _passengerLineContainer.style.display = DisplayStyle.None;
            _cellDataMatrix = null;
        }

        private void LoadLevelDataIntoUI(LevelDataSO levelData)
        {
            _xField.value = levelData.Rows;
            _yField.value = levelData.Cols;
            _busLineField.value = levelData.BusSequence.Count;
            _passengerLineField.value = levelData.PassengerLineCapacity;
            _timerLineField.value = levelData.Time;

            _cellDataMatrix = new CellSaveData[levelData.Rows, levelData.Cols];
            foreach (var cellData in levelData.GridCells)
            {
                _cellDataMatrix[cellData.coordinates.x, cellData.coordinates.y] = new CellSaveData
                {
                    coordinates = cellData.coordinates,
                    type = cellData.type,
                    occupant = cellData.occupant,
                    color = cellData.color
                };
            }

            RefreshGridUI(levelData.Rows, levelData.Cols);
            RefreshBusLineUI(levelData.BusSequence);
            SetPassengerLine(levelData.PassengerLineCapacity);
        }

        private void GenerateGrid(int x, int y)
        {
            _isEditMode = false;
            _currentLevelId = -1;
            _cellDataMatrix = new CellSaveData[x, y];

            for (int iX = 0; iX < x; iX++)
            for (int iY = 0; iY < y; iY++)
            {
                _cellDataMatrix[iX, iY] = new CellSaveData
                {
                    coordinates = new Vector2Int(iX, iY),
                    type = CellType.Walkable,
                    occupant = OccupantType.Empty,
                    color = EntityColor.Default
                };
            }

            RefreshGridUI(x, y);
        }

        private void RefreshBusLineUI(int count)
        {
            var sequence = new List<BusLineSaveData>();
            for (int i = 0; i < count; i++)
                sequence.Add(new BusLineSaveData { order = count - i, color = EntityColor.Default });

            RefreshBusLineUI(sequence);
        }

        private void RefreshBusLineUI(List<BusLineSaveData> busSequence)
        {
            _busLineContainer.Clear();
            _busSequenceList.Clear();
            _busLineContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            _busLineContainer.Add(new Label("Bus Line ➔")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold, marginLeft = 10, color = new Color(0.8f, 0.8f, 0.8f)
                }
            });

            var busColorField = new EnumField(_brushColorSelection) { style = { width = 60, marginRight = 10 } };
            EnumFieldLabelAdjust(busColorField);
            busColorField.RegisterValueChangedCallback(evt => _brushColorSelection = (EntityColor)evt.newValue);
            _busLineContainer.Add(busColorField);

            for (int i = 0; i < busSequence.Count; i++)
            {
                int index = i;
                var data = busSequence[index];
                _busSequenceList.Add(data);

                var busBtn = new Button
                {
                    text = $"{data.order}.",
                    style =
                    {
                        width = 35, height = 35, marginRight = 2, backgroundColor = new Color(0.15f, 0.15f, 0.15f)
                    }
                };
                UpdateBusButtonVisuals(busBtn, data.color);

                busBtn.clicked += () =>
                {
                    var entry = _busSequenceList[index];
                    entry.color = _brushColorSelection;
                    _busSequenceList[index] = entry;
                    UpdateBusButtonVisuals(busBtn, _brushColorSelection);
                };

                _busLineContainer.Add(busBtn);
            }
        }

        private void SetPassengerLine(int value)
        {
            if (value < 0) return;
            _passengerLineContainer.style.display = DisplayStyle.Flex;
            _passengerCountLabel.text = $"Passenger Line: {value}";
            _passengerCountLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            _passengerCountLabel.style.color = new Color(1f, 0.6f, 0f);
            _passengerLineField.value = value;
        }

        private void RefreshGridUI(int widthX, int widthY)
        {
            _gridContainer.Clear();
            for (int y = widthY - 1; y >= 0; y--)
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                for (int x = 0; x < widthX; x++)
                {
                    int posX = x, posY = y;
                    var cellLabel = new Label
                    {
                        style =
                        {
                            width = 40, height = 40, marginRight = 2, marginBottom = 2, fontSize = 9,
                            unityTextAlign = TextAnchor.MiddleCenter
                        }
                    };

                    cellLabel.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        if (evt.button != 0) return;
                        _isPainting = true;
                        OnCellClicked(posX, posY, cellLabel);
                        cellLabel.ReleasePointer(evt.pointerId);
                    });

                    cellLabel.RegisterCallback<PointerEnterEvent>(_ =>
                    {
                        if (_isPainting) OnCellClicked(posX, posY, cellLabel);
                    });

                    UpdateCellVisuals(cellLabel, _cellDataMatrix[posX, posY]);
                    row.Add(cellLabel);
                }

                _gridContainer.Add(row);
            }
        }

        private void OnCellClicked(int posX, int posY, Label label)
        {
            CellType cellType = _brushOccupant is OccupantType.ObstacleProp or OccupantType.Passenger
                ? CellType.Obstructed
                : CellType.Walkable;

            _cellDataMatrix[posX, posY].type = cellType;
            _cellDataMatrix[posX, posY].occupant = _brushOccupant;
            _cellDataMatrix[posX, posY].color =
                _brushOccupant == OccupantType.Passenger ? _brushColor : EntityColor.Default;

            UpdateCellVisuals(label, _cellDataMatrix[posX, posY]);
        }

        private void UpdateCellVisuals(Label label, CellSaveData data)
        {
            label.text = $"{data.coordinates.x},{data.coordinates.y}";
            Color defaultBg = new Color(0.3f, 0.3f, 0.3f);

            switch (data.occupant)
            {
                case OccupantType.ObstacleProp:
                    label.text = "OBS";
                    ApplyColorToElement(label, data.color, Color.black);
                    break;
                case OccupantType.Passenger:
                    label.text = "PSG";
                    ApplyColorToElement(label, data.color, defaultBg);
                    break;
                default:
                    label.style.backgroundColor = defaultBg;
                    label.style.color = Color.white;
                    break;
            }
        }

        private void UpdateBusButtonVisuals(Button btn, EntityColor color) =>
            ApplyColorToElement(btn, color, new Color(0.15f, 0.15f, 0.15f));

        private void ApplyColorToElement(VisualElement element, EntityColor color, Color fallbackBg)
        {
            if (ColorLookup.TryGetValue(color, out var colors))
            {
                element.style.backgroundColor = colors.bg;
                element.style.color = colors.text;
            }
            else
            {
                element.style.backgroundColor = fallbackBg;
                element.style.color = Color.white;
            }
        }

        #endregion

        #region UI Factory

        private VisualElement CreateEditExistingLevelSection()
        {
            var container = CreateSectionContainer("Enter Existing Level ID");
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 5 } };

            _existingLevelField = CreateCustomIntField(GetLevelNumberText, DefaultIntValue, 0.5f);
            SetFieldToVertical(_existingLevelField);

            row.Add(_existingLevelField);
            row.Add(CreateGenerateButton(GetLevelBtnText, () => GetDesiredLevel(_existingLevelField.value)));
            row.Add(CreateGenerateButton(DeleteSelectedLevelBtnText,
                () => DeleteDesiredLevel(_existingLevelField.value), "#FFFFFF", DeleteBtnBackgroundColorHex));
            container.Add(row);
            return container;
        }

        private VisualElement CreatePaletteSection()
        {
            var container = CreateSectionContainer("Brush Palette");
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 5 } };

            var occupantField = new EnumField("Occupant", _brushOccupant) { style = { flexGrow = 1, marginRight = 5 } };
            var colorField = new EnumField("Color", _brushColor) { style = { flexGrow = 1 } };
            EnumFieldLabelAdjust(occupantField);
            EnumFieldLabelAdjust(colorField);

            occupantField.RegisterValueChangedCallback(evt =>
            {
                _brushOccupant = (OccupantType)evt.newValue;
                bool needsColor = _brushOccupant == OccupantType.Passenger;
                colorField.SetEnabled(needsColor);
                if (!needsColor)
                {
                    _brushColor = EntityColor.Default;
                    colorField.value = _brushColor;
                }
            });
            colorField.RegisterValueChangedCallback(evt => _brushColor = (EntityColor)evt.newValue);
            colorField.SetEnabled(_brushOccupant == OccupantType.Passenger);

            row.Add(occupantField);
            row.Add(colorField);
            container.Add(row);
            return container;
        }

        private VisualElement CreateLevelSettingsSection()
        {
            var container = CreateSectionContainer("Level Info");
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 5 } };

            _xField = CreateCustomIntField("Row\n(x)", DefaultIntValue, 1);
            _yField = CreateCustomIntField("Column\n(y)", DefaultIntValue, 1);
            _busLineField = CreateCustomIntField("Bus\nLine Count", DefaultIntValue, 1);
            _passengerLineField = CreateCustomIntField("Passenger\nLine Count", DefaultIntValue, 1);
            _timerLineField = CreateCustomIntField("Timer\n(seconds)", DefaultTimerValue, 1);

            foreach (var field in new[] { _xField, _yField, _busLineField, _passengerLineField, _timerLineField })
            {
                SetFieldToVertical(field);
                row.Add(field);
            }

            container.Add(row);
            return container;
        }

        private VisualElement CreateActionButtonRow()
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row, paddingLeft = 5, paddingRight = 5,
                    justifyContent = Justify.SpaceBetween
                }
            };

            var gridBtn = CreateGenerateButton(GridBtnText, () => GenerateGrid(_xField.value, _yField.value));
            gridBtn.style.marginRight = 5;

            row.Add(gridBtn);
            row.Add(CreateGenerateButton(BusLineBtnText, () => RefreshBusLineUI(_busLineField.value)));
            row.Add(CreateGenerateButton(PassengerLineBtnText, () => SetPassengerLine(_passengerLineField.value)));
            return row;
        }

        private VisualElement CreateFooterSection()
        {
            var container = new VisualElement
            {
                style = { marginTop = 10, marginBottom = 10, paddingLeft = 5, paddingRight = 5 }
            };

            var saveBtn = CreateGenerateButton("Save Level", () => LevelSaveUtility.SaveLevel(
                    _cellDataMatrix, _busSequenceList, _passengerLineField.value,
                    SoDataPath, _isEditMode, _currentLevelId,
                    _timerLineField.value <= 0 ? 45 : _timerLineField.value),
                "#FFFFFF", SaveBtnBackgroundColorHex);

            saveBtn.style.height = 40;
            container.Add(saveBtn);
            return container;
        }

        private VisualElement CreatePathSection()
        {
            var container = CreateSectionContainer("Data Paths");
            container.Add(new ObjectField("LevelData(SO) Path:")
            {
                objectType = typeof(DefaultAsset),
                value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(SoDataPath)
            });
            container.SetEnabled(false);
            return container;
        }

        private VisualElement CreateHorizontalContainer(float marginTop, Color? bgColor = null,
            Justify justify = Justify.Center)
        {
            var container = new VisualElement
            {
                style =
                {
                    marginTop = marginTop, flexDirection = FlexDirection.Row, alignItems = Align.Center,
                    justifyContent = justify, paddingLeft = 10
                }
            };
            if (bgColor.HasValue) container.style.backgroundColor = bgColor.Value;
            return container;
        }

        private Button CreateGenerateButton(string label, System.Action onClick = null,
            string textColor = GridBtnTextColorHex, string bgColor = GridBtnBackgroundColorHex)
        {
            var btn = new Button(onClick ?? (() => { }))
            {
                text = label,
                style =
                {
                    height = 35, marginTop = 15, fontSize = 14, unityFontStyleAndWeight = FontStyle.Bold, flexGrow = 1
                }
            };
            if (ColorUtility.TryParseHtmlString(bgColor, out var bg)) btn.style.backgroundColor = bg;
            if (ColorUtility.TryParseHtmlString(textColor, out var txt)) btn.style.color = txt;
            return btn;
        }

        private static IntegerField CreateCustomIntField(string label, int value, float grow)
        {
            var field = new IntegerField(label)
            {
                value = value,
                style =
                {
                    flexGrow = grow, marginRight = 10, flexBasis = 0, flexDirection = FlexDirection.Row,
                    justifyContent = Justify.FlexStart
                }
            };
            field.labelElement.style.width = 65;
            field.labelElement.style.minWidth = 20;
            field.labelElement.style.flexGrow = 0;
            field.labelElement.style.marginRight = 2;

            var input = field.Q("unity-base-field__input");
            if (input != null)
            {
                input.style.width = 30;
                input.style.flexGrow = 0;
                input.style.marginRight = 2;
            }

            return field;
        }

        private static VisualElement CreateHeaderSection(string title)
        {
            var label = new Label(title)
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 20, marginTop = 10, marginBottom = 10 }
            };
            if (ColorUtility.TryParseHtmlString(TitleColorHex, out var c)) label.style.color = c;
            return label;
        }

        private static VisualElement CreateSectionContainer(string title)
        {
            var section = new VisualElement { style = { marginBottom = 10, paddingLeft = 5 } };
            section.Add(new Label(title)
                { style = { unityFontStyleAndWeight = FontStyle.Bold, marginBottom = 5, marginTop = 5 } });
            return section;
        }

        private void EnumFieldLabelAdjust(EnumField field)
        {
            var label = field.labelElement;
            if (label == null) return;
            label.style.minWidth = 0;
            label.style.width = StyleKeyword.Auto;
            label.style.marginRight = 5;
        }

        private void SetFieldToVertical(VisualElement field)
        {
            field.style.flexDirection = FlexDirection.Column;
            var label = field.Q<Label>();
            if (label == null) return;
            label.style.width = StyleKeyword.Auto;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.minWidth = 0;
        }

        #endregion
    }
}