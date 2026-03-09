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

        #region Colors

        private const string TitleColorHex = "#FF9F00";
        private const string GridBtnTextColorHex = "#FFFFFF";
        private const string GridBtnBackgroundColorHex = "#00C2FF";
        private const string SaveBtnBackgroundColorHex = "#39B33F";
        private const string DeleteBtnBackgroundColorHex = "#E55353";

        #endregion

        #region Path

        // private const string JsonDataPath = "Assets/Resources/Data/JSON_Level_Data";
        private const string SoDataPath = "Assets/Resources/Data/SO_Level_Data";

        #endregion

        #region Strings

        private const string HeaderTitleText = "Bus Jam Level Creator";
        private const string HelpBoxText = "Enter parameters to initialize grid.";
        private const string GridBtnText = "Draw Grid";
        private const string BusLineBtnText = "Draw Bus Line";
        private const string PassengerLineBtnText = "Set Passenger Line";
        private const string GetLevelNumberText = "Existing Level Id";
        private const string GetLevelBtnText = "Get Existing Level";
        private const string DeleteSelectedLevelBtnText = "Delete Selected Level";

        #endregion

        #region Default UI values

        private const int DefaultIntValue = 5;
        private const int DefaultTimerValue = 45;
        private const float DefaultMargin = 10f;

        #endregion

        #region Fields

        private IntegerField _xField, _yField, _busLineField, _passengerLineField, _timerLineField, _existingLevelField;

        private CellSaveData[,] _cellDataMatrix;
        private List<BusLineSaveData> _busSequenceList = new List<BusLineSaveData>();

        private OccupantType _brushOccupant = OccupantType.Empty;
        private EntityColor _brushColor = EntityColor.Default;
        private EntityColor _bushColorSelection = EntityColor.Default;
        private bool _isPainting = false;

        private bool _isEditMode = false;
        private int _currentLevelId = -1;

        private static readonly Dictionary<EntityColor, (Color bg, Color Text)> ColorLookup = new()
        {
            { EntityColor.White, (Color.white, Color.black) },
            { EntityColor.Blue, (Color.blue, Color.white) },
            { EntityColor.Yellow, (Color.yellow, Color.black) },
            { EntityColor.Green, (Color.green, Color.white) },
            { EntityColor.Pink, (Color.magenta, Color.white) },
            { EntityColor.Default, (new Color(0.15f, 0.15f, 0.15f), Color.white) }
        };

        private ScrollView _mainScrollView;
        private VisualElement _busLineContainer;
        private VisualElement _passengerLineContainer;
        private Label _passengerCountLabel;
        private VisualElement _gridContainer;
        private VisualElement _createLevel;

        #endregion

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
                if (evt.button == 0)
                {
                    _isPainting = false;
                }
            });
        }

        #region Setup Methods

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

        private VisualElement CreateEditExistingLevelSection()
        {
            var container = CreateSectionContainer("Enter Existing Level ID");

            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 5 } };
            _existingLevelField = CreateCustomIntField(GetLevelNumberText, DefaultIntValue, 0.5f);
            SetFieldToVertical(_existingLevelField);
            var getBtn = CreateGenerateButton(GetLevelBtnText, () => GetDesiredLevel(_existingLevelField.value));

            var deleteBtn = CreateGenerateButton(DeleteSelectedLevelBtnText,
                () => DeleteDesiredLevel(_existingLevelField.value), "#FFFFFF", DeleteBtnBackgroundColorHex);

            row.Add(_existingLevelField);
            row.Add(getBtn);
            row.Add(deleteBtn);
            container.Add(row);
            return container;
        }


        private VisualElement CreatePaletteSection()
        {
            var container = CreateSectionContainer("Brush Palette");
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 5 } };
            var occupantField = new EnumField("Occupant", _brushOccupant) { style = { flexGrow = 1, marginRight = 5 } };
            EnumFieldLabelAdjust(occupantField);
            var colorField = new EnumField("Color", _brushColor) { style = { flexGrow = 1 } };
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

        private void EnumFieldLabelAdjust(EnumField field)
        {
            var label = field.labelElement;
            if (label != null)
            {
                label.style.minWidth = 0;
                label.style.width = StyleKeyword.Auto;
                label.style.marginRight = 5;
            }
        }

        private void SetupDrawingArea(VisualElement root)
        {
            _mainScrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal)
            {
                style =
                {
                    marginTop = DefaultMargin, flexGrow = 1, borderTopWidth = 1,
                    borderTopColor = new Color(0.15f, 0.15f, 0.15f)
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

        #region Action Methods

        private void GetDesiredLevel(int levelNumber)
        {
            LevelDataSO loadedLevel = LevelSaveUtility.GetSelectedLevel(levelNumber, SoDataPath);
            if (loadedLevel)
            {
                _isEditMode = true;
                _currentLevelId = levelNumber;
                LoadLevelDataIntoUI(loadedLevel);
            }
        }


        private void DeleteDesiredLevel(int levelNumber)
        {
            bool isDeleted = LevelSaveUtility.DeleteSelectedLevel(levelNumber, SoDataPath);

            if (isDeleted && _isEditMode && _currentLevelId == levelNumber)
            {
                _isEditMode = false;
                _gridContainer.Clear();
                _busLineContainer.Clear();
                _passengerLineContainer.style.display = DisplayStyle.None;
                _cellDataMatrix = null;
                Debug.Log("Deleted currently editing level. UI cleared.");
            }
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
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    _cellDataMatrix[i, j] = new CellSaveData
                    {
                        coordinates = new Vector2Int(i, j),
                        type = CellType.Walkable,
                        occupant = OccupantType.Empty,
                        color = EntityColor.Default
                    };
                }
            }

            RefreshGridUI(x, y);
        }

        private void RefreshBusLineUI(int count)
        {
            _busLineContainer.Clear();
            _busSequenceList.Clear();
            _busLineContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            var busLineLabel = new Label("Bus Line ➔")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginLeft = 10,
                    color = new Color(0.8f, 0.8f, 0.8f),
                }
            };
            _busLineContainer.Add(busLineLabel);

            var busColorField = new EnumField(_bushColorSelection)
            {
                style = { width = 60, marginRight = 10 }
            };
            EnumFieldLabelAdjust(busColorField);

            busColorField.RegisterValueChangedCallback(evt => { _bushColorSelection = (EntityColor)evt.newValue; });
            _busLineContainer.Add(busColorField);

            for (var i = 0; i < count; i++)
            {
                var index = i;
                var reverseOrder = count - index;
                _busSequenceList.Add(new BusLineSaveData()
                {
                    order = reverseOrder,
                    color = EntityColor.Default,
                });

                Button busBtn = new Button(() => Debug.LogWarning($"Bus Index: {index}"))
                {
                    text = $"{reverseOrder}.",
                    style =
                    {
                        width = 35, height = 35, marginRight = 2, backgroundColor = new Color(0.15f, 0.15f, 0.15f)
                    }
                };
                busBtn.clicked += () =>
                {
                    var data = _busSequenceList[index];
                    data.color = _bushColorSelection;
                    _busSequenceList[index] = data;

                    UpdateBusButtonVisuals(busBtn, _bushColorSelection);
                };

                _busLineContainer.Add(busBtn);
            }
        }

        private void RefreshBusLineUI(List<BusLineSaveData> loadedBusSequence)
        {
            int count = loadedBusSequence.Count;
            _busLineContainer.Clear();
            _busSequenceList.Clear();
            _busLineContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            var busLineLabel = new Label("Bus Line ➔")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginLeft = 10,
                    color = new Color(0.8f, 0.8f, 0.8f),
                }
            };
            _busLineContainer.Add(busLineLabel);

            var busColorField = new EnumField(_bushColorSelection)
            {
                style = { width = 60, marginRight = 10 }
            };
            EnumFieldLabelAdjust(busColorField);

            busColorField.RegisterValueChangedCallback(evt => { _bushColorSelection = (EntityColor)evt.newValue; });
            _busLineContainer.Add(busColorField);

            for (var i = 0; i < count; i++)
            {
                var index = i;
                var loadedData = loadedBusSequence[index];
                _busSequenceList.Add(new BusLineSaveData()
                {
                    order = loadedData.order,
                    color = EntityColor.Default,
                });

                Button busBtn = new Button(() => Debug.LogWarning($"Bus Index: {index}"))
                {
                    text = $"{loadedData.order}.",
                    style =
                    {
                        width = 35, height = 35, marginRight = 2, backgroundColor = new Color(0.15f, 0.15f, 0.15f)
                    }
                };

                UpdateBusButtonVisuals(busBtn, loadedData.color);
                busBtn.clicked += () =>
                {
                    var data = _busSequenceList[index];
                    data.color = _bushColorSelection;
                    _busSequenceList[index] = data;

                    UpdateBusButtonVisuals(busBtn, _bushColorSelection);
                };

                _busLineContainer.Add(busBtn);
            }
        }

        private void UpdateBusButtonVisuals(Button btn, EntityColor color)
        {
            Color defaultBgColor = new Color(0.15f, 0.15f, 0.15f);

            ApplyColorToElement(btn, color, defaultBgColor);
        }

        private void ApplyColorToElement(VisualElement element, EntityColor color, Color defaultBgColor)
        {
            if (ColorLookup.TryGetValue(color, out var colors))
            {
                element.style.backgroundColor = colors.bg;
                element.style.color = colors.Text;
            }
            else
            {
                element.style.backgroundColor = defaultBgColor;
                element.style.color = Color.white;
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


        private void RefreshGridUI(int rows, int cols)
        {
            _gridContainer.Clear();
            for (int y = cols - 1; y >= 0; y--)
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                for (int x = 0; x < rows; x++)
                {
                    int posX = x;
                    int posY = y;

                    Label cellBtnLabel = new Label
                    {
                        style =
                        {
                            width = 40,
                            height = 40,
                            marginRight = 2,
                            marginBottom = 2,
                            fontSize = 9,
                            unityTextAlign = TextAnchor.MiddleCenter
                        }
                    };

                    cellBtnLabel.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        if (evt.button == 0)
                        {
                            _isPainting = true;
                            OnCellClicked(posX, posY, cellBtnLabel);

                            // UI Toolkit'te butonlar tıklandığında fareyi kilitler (Capture).
                            // Sürükleme yapabilmek için bu kilidi serbest bırakmamız gerekir.
                            cellBtnLabel.ReleasePointer(evt.pointerId);
                        }
                    });
                    cellBtnLabel.RegisterCallback<PointerEnterEvent>(evt =>
                    {
                        if (_isPainting)
                        {
                            OnCellClicked(posX, posY, cellBtnLabel);
                        }
                    });

                    UpdateButtonVisuals(cellBtnLabel, _cellDataMatrix[posX, posY]);

                    row.Add(cellBtnLabel);
                }

                _gridContainer.Add(row);
            }
        }


        private void OnCellClicked(int posX, int posY, Label clickedBtn)
        {
            CellType calculatedCellType =
                (_brushOccupant == OccupantType.ObstacleProp || _brushOccupant == OccupantType.Passenger)
                    ? CellType.Obstructed
                    : CellType.Walkable;

            EntityColor finalColor = (_brushOccupant == OccupantType.Passenger) ? _brushColor : EntityColor.Default;

            _cellDataMatrix[posX, posY].type = calculatedCellType;
            _cellDataMatrix[posX, posY].occupant = _brushOccupant;
            _cellDataMatrix[posX, posY].color = finalColor;

            UpdateButtonVisuals(clickedBtn, _cellDataMatrix[posX, posY]);
        }

        private void UpdateButtonVisuals(Label label, CellSaveData data)
        {
            label.text = $"{data.coordinates.x},{data.coordinates.y}";
            Color defaultBgColor = new Color(0.3f, 0.3f, 0.3f);

            if (data.occupant == OccupantType.ObstacleProp)
            {
                label.text = "OBS";
                defaultBgColor = Color.black;
                label.style.color = Color.white;
                ApplyColorToElement(label, data.color, defaultBgColor);
            }
            else if (data.occupant == OccupantType.Passenger)
            {
                label.text = "PSG";
                ApplyColorToElement(label, data.color, defaultBgColor);
            }

            else
            {
                label.style.backgroundColor = defaultBgColor;
                label.style.color = Color.white;
            }
        }

        #endregion


        #region UI Factory Methods

        private VisualElement CreateHorizontalContainer(float marginTop, Color? bgColor = null,
            Justify justifyAlignParam = Justify.Center)
        {
            var container = new VisualElement
            {
                style =
                {
                    marginTop = marginTop,
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    justifyContent = justifyAlignParam,
                    paddingLeft = 10
                }
            };

            if (bgColor.HasValue)
            {
                container.style.backgroundColor = bgColor.Value;
            }

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

            Button levelGridBtn = CreateGenerateButton(GridBtnText, () => GenerateGrid(_xField.value, _yField.value));
            levelGridBtn.style.marginRight = 5;

            Button busLineBtn =
                CreateGenerateButton(BusLineBtnText, () => RefreshBusLineUI(_busLineField.value));

            Button passengerLineCountBtn = CreateGenerateButton(PassengerLineBtnText,
                () => SetPassengerLine(_passengerLineField.value));
            row.Add(levelGridBtn);
            row.Add(busLineBtn);
            row.Add(passengerLineCountBtn);
            return row;
        }

        private Button CreateGenerateButton(string buttonName, System.Action onClick = null,
            string btnTextColor = GridBtnTextColorHex,
            string btnBgColor = GridBtnBackgroundColorHex)
        {
            var btn = new Button(onClick ?? (() => { }))
            {
                text = buttonName,
                style =
                {
                    height = 35, marginTop = 15, fontSize = 14, unityFontStyleAndWeight = FontStyle.Bold, flexGrow = 1
                }
            };
            if (ColorUtility.TryParseHtmlString(btnBgColor, out Color bg))
                btn.style.backgroundColor = bg;
            if (ColorUtility.TryParseHtmlString(btnTextColor, out Color txt)) btn.style.color = txt;

            return btn;
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

            // container.Add(new ObjectField("JSON Path")
            // {
            //     objectType = typeof(DefaultAsset),
            //     value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(JsonDataPath)
            // });


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

            SetFieldToVertical(_xField);
            SetFieldToVertical(_yField);
            SetFieldToVertical(_busLineField);
            SetFieldToVertical(_passengerLineField);
            SetFieldToVertical(_timerLineField);

            row.Add(_xField);
            row.Add(_yField);
            row.Add(_busLineField);
            row.Add(_passengerLineField);
            row.Add(_timerLineField);
            container.Add(row);

            return container;
        }

        private void SetFieldToVertical(VisualElement field)
        {
            field.style.flexDirection = FlexDirection.Column;
            var label = field.Q<Label>();
            if (label != null)
            {
                label.style.width = StyleKeyword.Auto;
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.style.minWidth = 0;
            }
        }


        private static IntegerField CreateCustomIntField(string labelParam, int val, float grow)
        {
            var field = new IntegerField(labelParam)
            {
                value = val,
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
            var inputElement = field.Q("unity-base-field__input");
            if (inputElement == null) return field;
            inputElement.style.width = 30;
            inputElement.style.flexGrow = 0;
            inputElement.style.marginRight = 2;

            return field;
        }

        private static VisualElement CreateHeaderSection(string titleParam)
        {
            var label = new Label(titleParam)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 20,
                    marginTop = 10,
                    marginBottom = 10
                }
            };
            if (ColorUtility.TryParseHtmlString(TitleColorHex, out var c))
                label.style.color = c;

            return label;
        }


        private static VisualElement CreateSectionContainer(string titleParam)
        {
            var section = new VisualElement { style = { marginBottom = 10, paddingLeft = 5 } };
            section.Add(
                new Label(titleParam)
                    { style = { unityFontStyleAndWeight = FontStyle.Bold, marginBottom = 5, marginTop = 5 } });
            return section;
        }


        private VisualElement CreateFooterSection()
        {
            var container = new VisualElement
            {
                style = { marginTop = 10, marginBottom = 10, paddingLeft = 5, paddingRight = 5 }
            };
            Button saveBtn = CreateGenerateButton("Save Level", () => LevelSaveUtility.SaveLevel(
                    _cellDataMatrix,
                    _busSequenceList,
                    _passengerLineField.value,
                    SoDataPath,
                    _isEditMode,
                    _currentLevelId, _timerLineField.value <= 0 ? 45 : _timerLineField.value), "#FFFFFF",
                SaveBtnBackgroundColorHex);
            saveBtn.style.height = 40;
            container.Add(saveBtn);

            return container;
        }

        #endregion
    }
}